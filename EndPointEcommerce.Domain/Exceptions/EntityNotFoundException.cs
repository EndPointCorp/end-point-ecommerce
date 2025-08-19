// Copyright 2025 End Point Corporation. Apache License, version 2.0.

namespace EndPointEcommerce.Domain.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException() { }
    public EntityNotFoundException(string message) : base(message) { }
    public EntityNotFoundException(string message, Exception inner) : base(message, inner) { }
}
