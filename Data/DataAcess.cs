//using Microsoft.AspNetCore.Identity;
//using StoreQR.Interface;
//using StoreQR.Models;
//using System.Data;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Infrastructure;

//namespace StoreQR.Data
//{
//    public class DataAccess : IDataAccess
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly UserManager<IdentityUser> _userManager;
//        private readonly SignInManager<IdentityUser> _signInManager;

//        public DataAccess(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
//        {
//            _context = context ?? throw new ArgumentNullException(nameof(context));
//            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
//            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
//        }

       
//        //Radera all användarinfo för användare
//        public async Task<bool> DeleteAllUserInfoByUserId(string userId, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
//        {
//            using (var command = _context.Database.GetDbConnection().CreateCommand())
//            {
//                command.CommandText = "DeleteAllUserInfoByUserId";
//                command.CommandType = CommandType.StoredProcedure;

//                var userInfoParameter = command.CreateParameter();
//                userInfoParameter.ParameterName = "@UserId";
//                userInfoParameter.DbType = DbType.String;
//                userInfoParameter.Value = userId;
//                command.Parameters.Add(userInfoParameter);

//                await _context.Database.OpenConnectionAsync();

//                using (var transaction = _context.Database.GetDbConnection().BeginTransaction())
//                {
//                    command.Transaction = transaction;

//                    try
//                    {
//                        //Radera användaren
//                        var user = await userManager.FindByIdAsync(userId);
//                        if (user != null)
//                        {

//                            //Radera användaren från microsoft identity
//                            var result = await userManager.DeleteAsync(user);

//                            if (!result.Succeeded)
//                            {
//                                throw new InvalidOperationException($"Unexpected error occurred deleting user from Microsoft Identity.");
//                            }

//                            //Logga ut användaren
//                            await signInManager.SignOutAsync();
//                        }

//                        //Utför stored procedure som raderar alla rader som har detta userid som raderas
//                        await command.ExecuteNonQueryAsync();

//                        transaction.Commit();
//                        return true; //Operationen lyckades
//                    }
//                    catch (Exception)
//                    {
//                        transaction.Rollback();
//                        return false; //Operationen misslyckades
//                    }
//                    finally
//                    {
//                        await _context.Database.CloseConnectionAsync();
//                    }
//                }

//            }
//        }

//    }
//}
