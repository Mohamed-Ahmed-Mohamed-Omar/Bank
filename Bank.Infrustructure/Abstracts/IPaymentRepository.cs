﻿using Bank.Data.Entities;
using Bank.InfrastructureBases;

namespace Bank.Infrustructure.Abstracts
{
    public interface IPaymentRepository : IGenericRepositoryAsync<Payment>
    {
    }
}
