// Copyright 2025 End Point Corporation. Apache License, version 2.0.

namespace EndPointEcommerce.WebApi.ResourceModels;

public class OrderPost
{
    public string? PaymentMethodNonceValue { get; set; }
    public string? PaymentMethodNonceDescriptor { get; set; }
}
