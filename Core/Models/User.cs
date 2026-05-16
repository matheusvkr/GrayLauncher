using System;

namespace Client.Core.Models;

public class User
{
    public User()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string HashPassword { get; set; } = string.Empty;
}
