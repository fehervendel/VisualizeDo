﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VisualizeDo.Context;
using VisualizeDo.Models;

namespace VisualizeDo.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<IdentityUser> _userManager;
    
    public UserRepository(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }
    
    public async Task<IEnumerable<User>> GetAll()
    {
        using var dbContext = new VisualizeDoContext();
        return await dbContext.Users.ToListAsync();
    }

    public async Task<User?> GetById(int id)
    {
        using var dbContext = new VisualizeDoContext();
        return await dbContext.Users.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task Add(User user)
    {
        using var dbContext = new VisualizeDoContext();
        dbContext.Add(user);
        await dbContext.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        using var dbContext = new VisualizeDoContext();
        
        User? userToDelete = await dbContext.Users.FirstOrDefaultAsync(e => e.Id == id);
        if (userToDelete != null)
        {
            var stringId = userToDelete?.IdentityUserId;
            var identityUser = await _userManager.FindByIdAsync(stringId);
            
            userToDelete.IdentityUserId = null;
            
            await dbContext.SaveChangesAsync();

            var result = await _userManager.DeleteAsync(identityUser);

            dbContext.Remove(userToDelete);

            await dbContext.SaveChangesAsync();
        }
    }

    public async Task Update(User user)
    {
        using var dbContext = new VisualizeDoContext();
        dbContext.Update(user);
        await dbContext.SaveChangesAsync();
    }
}