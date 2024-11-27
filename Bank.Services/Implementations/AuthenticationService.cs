using Bank.Data.Entities.Identity;
using Bank.Data.Helpers;
using Bank.Infrustructure.Context;
using Bank.Services.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Bank.Services.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailsService _emailsService;
        private readonly ApplicationDbContext _context;
        private readonly IUrlHelper _urlHelper;
        private readonly JWT _jwt;

        public AuthenticationService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor, IEmailsService emailsService, IUrlHelper urlHelper, IOptions<JWT> jwt, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _emailsService = emailsService;
            _urlHelper = urlHelper;
            _jwt = jwt.Value;
            _context = context;
        }

        public async Task<AuthModel> RegisterAsync(ApplicationUser model, string Password)
        {
            var trans = await _context.Database.BeginTransactionAsync();
            try
            {
                if (await _userManager.FindByEmailAsync(model.Email) is not null)
                    return (new AuthModel { Message = "Email is already registered!" });

                if (await _userManager.FindByNameAsync(model.UserName) is not null)
                    return (new AuthModel { Message = "Username is already registered!" });

                // Check if the CreditCardNumber is already registered
                var isCreditCardExists = await _userManager.Users.AnyAsync(u => u.CreditCardNumber == model.CreditCardNumber);
                if (isCreditCardExists)
                    return new AuthModel { Message = "Credit card number is already registered!" };


                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    CreditCardNumber = model.CreditCardNumber,
                    RegistrationDate = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, Password);

                if (!result.Succeeded)
                {
                    var errors = string.Empty;

                    foreach (var error in result.Errors)
                        errors += $"{error.Description},";

                    return (new AuthModel { Message = errors });
                }

                await _userManager.AddToRoleAsync(user, "User");

                var jwtSecurityToken = await CreateJwtToken(user);

                var refreshToken = GenerateRefreshToken();

                //Send Confirm Email

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var requestAccessor = _httpContextAccessor.HttpContext.Request;

                var returnUrl = requestAccessor.Scheme + "://" + requestAccessor.Host + _urlHelper.Action("ConfirmEmail", "Authentication", new { code = code, Email = user.Email });

                var message = $"To Confirm Email Click Link: <a href='{returnUrl}'>Link Of Confirmation</a>";

                var sent = await _emailsService.SendEmail(user.Email, message, "Confirm Email");

                if (sent != "Success")
                {
                    return (new AuthModel { Message = "Email Not Confirm!" });
                }

                user.RefreshTokens?.Add(refreshToken);
                await _userManager.UpdateAsync(user);

                await trans.CommitAsync();
                return (new AuthModel
                {
                    Email = user.Email,
                    ExpiresOn = jwtSecurityToken.ValidTo,
                    IsAuthenticated = true,
                    Roles = new List<string> { "User" },
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                    Username = user.UserName,
                    RefreshToken = refreshToken.Token,
                    RefreshTokenExpiration = refreshToken.ExpiresOn
                });
            }
            catch (Exception ex)
            {
                await trans.RollbackAsync();
                return (new AuthModel
                {
                    Message = ex.Message
                });
            }
        }

        public async Task<AuthModel> RegisterAsync(ApplicationUser model, string RoleName, string Password)
        {
            var trans = await _context.Database.BeginTransactionAsync();
            try
            {
                if (await _userManager.FindByEmailAsync(model.Email) is not null)
                    return (new AuthModel { Message = "Email is already registered!" });

                if (await _userManager.FindByNameAsync(model.UserName) is not null)
                    return (new AuthModel { Message = "Username is already registered!" });

                // Check if the CreditCardNumber is already registered
                var isCreditCardExists = await _userManager.Users.AnyAsync(u => u.CreditCardNumber == model.CreditCardNumber);
                if (isCreditCardExists)
                    return new AuthModel { Message = "Credit card number is already registered!" };

                if (!IsPasswordStrong(Password))
                    return new AuthModel { Message = "Password must be at least 8 characters long, include uppercase, lowercase, a digit, and a special character." };

                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    CreditCardNumber = model.CreditCardNumber,
                    RegistrationDate = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, Password);

                if (!result.Succeeded)
                {
                    var errors = string.Empty;

                    foreach (var error in result.Errors)
                        errors += $"{error.Description},";

                    return (new AuthModel { Message = errors });
                }

                if (await _roleManager.RoleExistsAsync(RoleName))
                    await _userManager.AddToRoleAsync(user, RoleName);
                else
                    return (new AuthModel { Message = "InVaild Role" });

                var jwtSecurityToken = await CreateJwtToken(user);

                var refreshToken = GenerateRefreshToken();

                //Send Confirm Email

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var requestAccessor = _httpContextAccessor.HttpContext.Request;

                var returnUrl = requestAccessor.Scheme + "://" + requestAccessor.Host + _urlHelper.Action("ConfirmEmail", "Authentication", new { code = code, Email = user.Email });

                var message = $"To Confirm Email Click Link: <a href='{returnUrl}'>Link Of Confirmation</a>";

                var sent = await _emailsService.SendEmail(user.Email, message, "Confirm Email");

                if (sent != "Success")
                {
                    return (new AuthModel { Message = "Email Not Confirm!" });
                }

                user.RefreshTokens?.Add(refreshToken);
                await _userManager.UpdateAsync(user);

                await trans.CommitAsync();
                return (new AuthModel
                {
                    Email = user.Email,
                    ExpiresOn = jwtSecurityToken.ValidTo,
                    IsAuthenticated = true,
                    Roles = new List<string> { "User" },
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                    Username = user.UserName,
                    RefreshToken = refreshToken.Token,
                    RefreshTokenExpiration = refreshToken.ExpiresOn
                });
            }
            catch (Exception ex)
            {
                await trans.RollbackAsync();
                return (new AuthModel
                {
                    Message = ex.Message
                });
            }
        }

        public async Task<AuthModel> LoginAsync(string model, string Password)
        {
            var authModel = new AuthModel();

            var user = await _userManager.FindByEmailAsync(model) ??
                        await _userManager.FindByNameAsync(model);

            if (user is null || !await _userManager.CheckPasswordAsync(user, Password))
            {
                authModel.Message = "Email or Password is incorrect!";
                return (authModel);
            }

            if (!user.EmailConfirmed)
            {
                var timeSinceRegistration = DateTime.UtcNow - user.RegistrationDate;
                if (timeSinceRegistration.TotalHours >= 24)
                {
                    var result = await _userManager.DeleteAsync(user);
                    authModel.Message = "Email is not confirmed! And User deleted successfully!";
                    return authModel;
                }
                else
                {
                    authModel.Message = "Email is not confirmed! User will be deleted if not confirmed within 24 hours.";
                    return authModel;
                }
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Roles = rolesList.ToList();

            if (user.RefreshTokens.Any(t => t.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                authModel.RefreshToken = activeRefreshToken.Token;
                authModel.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
            }
            else
            {
                var refreshToken = GenerateRefreshToken();
                authModel.RefreshToken = refreshToken.Token;
                authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }

            return (authModel);
        }

        public async Task<string> ConfirmEmail(string Email, string code)
        {
            if (Email == null || code == null)
                return "ErrorWhenConfirmEmail";
            var user = await _userManager.FindByEmailAsync(Email);
            var confirmEmail = await _userManager.ConfirmEmailAsync(user, code);
            if (!confirmEmail.Succeeded)
                return "ErrorWhenConfirmEmail";
            return "Success";
        }

        public async Task<string> SendResetPasswordAsync(string Email)
        {
            var trans = await _context.Database.BeginTransactionAsync();
            try
            {
                //user
                var user = await _userManager.FindByEmailAsync(Email);
                //user not Exist => not found
                if (user == null)
                    return "UserNotFound";
                //Generate Random Number

                //Random generator = new Random();
                //string randomNumber = generator.Next(0, 1000000).ToString("D6");
                var chars = "0123456789";
                var random = new Random();
                var randomNumber = new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());

                //update User In Database Code
                user.Code = randomNumber;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                    return "ErrorInUpdateUser";
                var message = "Code To Reset Passsword : " + user.Code;
                //Send Code To  Email 
                await _emailsService.SendEmail(user.Email, message, "Reset Password");
                await trans.CommitAsync();
                return "Success";
            }
            catch (Exception ex)
            {
                await trans.RollbackAsync();
                return "Failed";
            }
        }

        public async Task<string> ConfirmResetPasswordAsync(string Code, string Email)
        {
            //Get User
            //user
            var user = await _userManager.FindByEmailAsync(Email);
            //user not Exist => not found
            if (user == null)
                return "UserNotFound";
            //Decrept Code From Database User Code
            var userCode = user.Code;
            //Equal With Code
            if (userCode == Code) return "Success";
            return "Failed";
        }

        public async Task<string> ResetPasswordAsync(string Email, string Password)
        {
            var trans = await _context.Database.BeginTransactionAsync();
            try
            {
                //Get User
                var user = await _userManager.FindByEmailAsync(Email);
                //user not Exist => not found
                if (user == null)
                    return "UserNotFound";
                await _userManager.RemovePasswordAsync(user);
                if (!await _userManager.HasPasswordAsync(user))
                {
                    await _userManager.AddPasswordAsync(user, Password);
                }
                await trans.CommitAsync();
                return "Success";
            }
            catch (Exception ex)
            {
                await trans.RollbackAsync();
                return "Failed";
            }
        }

        #region Helpers

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }

        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using var generator = new RNGCryptoServiceProvider();

            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow
            };
        }

        private bool IsPasswordStrong(string password)
        {
            if (password.Length < 8) return false;
            if (!password.Any(char.IsUpper)) return false; // At least one uppercase letter
            if (!password.Any(char.IsLower)) return false; // At least one lowercase letter
            if (!password.Any(char.IsDigit)) return false; // At least one digit
            if (!password.Any(ch => "!@#$%^&*()_+[]{}|;:',.<>?/`~".Contains(ch))) return false; // At least one special character
            return true;
        }

        #endregion
    }
}
