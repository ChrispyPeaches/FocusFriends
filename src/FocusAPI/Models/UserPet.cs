using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusAPI.Models;

public class UserPet : FocusCore.Models.BaseUserPet
{
    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new User? User { get; set; }

    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new Pet? Pet { get; set; }
}
