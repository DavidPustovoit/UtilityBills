using System;

namespace Var14.Core.Domain;

public sealed record Subscriber
{
    public Subscriber(string id, string fullName, string email)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Id не може бути порожнім");
        }

        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("Ім'я не може бути порожнім");
        }

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            throw new ArgumentException("Некоректний формат email");
        }

        Id = id;
        FullName = fullName;
        Email = email;
    }

    public string Id { get; }
    public string FullName { get; }
    public string Email { get; }
}
