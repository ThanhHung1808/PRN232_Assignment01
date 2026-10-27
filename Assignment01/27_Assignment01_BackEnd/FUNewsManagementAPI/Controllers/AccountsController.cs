using FUNewsManagementAPI.DTOs;
using FUNewsManagementAPI.Models;
using FUNewsManagementAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FUNewsManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ODataController
    {
        private readonly ISystemAccountRepository _accountRepository;

        public AccountsController(ISystemAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<AccountResponseDto>>> GetAccounts()
        {
            var accounts = await _accountRepository.GetAccountsAsync();
            var dtos = accounts.Select(a => new AccountResponseDto
            {
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                AccountEmail = a.AccountEmail,
                AccountRole = a.AccountRole,
                CreatedArticlesCount = a.CreatedNewsArticles?.Count ?? 0
            });
            return Ok(dtos.AsQueryable());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SystemAccount>> GetAccount(short id)
        {
            var account = await _accountRepository.GetAccountByIdAsync(id);
            if (account == null)
            {
                return NotFound(new { message = $"Account with ID {id} not found." });
            }
            return Ok(account);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] AccountCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var account = new SystemAccount
                {
                    AccountId = dto.AccountId,
                    AccountName = dto.AccountName,
                    AccountEmail = dto.AccountEmail,
                    AccountRole = dto.AccountRole,
                    AccountPassword = dto.AccountPassword
                };

                var created = await _accountRepository.AddAccountAsync(account);
                return CreatedAtAction(nameof(GetAccount), new { id = created.AccountId }, created);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(short id, [FromBody] AccountCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var account = new SystemAccount
                {
                    AccountId = id,
                    AccountName = dto.AccountName,
                    AccountEmail = dto.AccountEmail,
                    AccountRole = dto.AccountRole,
                    AccountPassword = dto.AccountPassword
                };

                var updated = await _accountRepository.UpdateAccountAsync(account);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(short id)
        {
            try
            {
                var hasArticles = await _accountRepository.HasCreatedArticlesAsync(id);
                if (hasArticles)
                {
                    return BadRequest(new { message = "Cannot delete this account because it has created news articles." });
                }

                var success = await _accountRepository.DeleteAccountAsync(id);
                if (!success)
                {
                    return NotFound(new { message = $"Account with ID {id} not found." });
                }

                return Ok(new { message = "Account deleted successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("profile/{id}")]
        public async Task<IActionResult> UpdateProfile(short id, [FromBody] ProfileUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existing = await _accountRepository.GetAccountByIdAsync(id);
                if (existing == null)
                {
                    return NotFound(new { message = $"Account with ID {id} not found." });
                }

                existing.AccountName = dto.AccountName;
                existing.AccountEmail = dto.AccountEmail;
                if (!string.IsNullOrWhiteSpace(dto.NewPassword))
                {
                    existing.AccountPassword = dto.NewPassword;
                }

                var updated = await _accountRepository.UpdateAccountAsync(existing);
                return Ok(new { message = "Profile updated successfully.", account = updated });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
