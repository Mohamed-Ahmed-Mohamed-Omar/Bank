﻿using Bank.Data.Entities;

namespace Bank.Infrustructure.Abstracts
{
    public interface IPaymentRepository
    {
        Task<Payment> PaymentAsync(Payment paymentm, string username);
        Task<IQueryable<Payment>> GetAllPaymentsAsync(string? username); // Task<IQueryable<Payment>> GetAllPaymentsAsync();
    }
}
