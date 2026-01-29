using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickLoan.Web.Interfaces;
using QuickLoan.Web.Models;

namespace QuickLoan.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IApiService _apiService;
    private readonly IAuthService _authService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(IApiService apiService, IAuthService authService, ILogger<AdminController> logger)
    {
        _apiService = apiService;
        _authService = authService;
        _logger = logger;
    }

    // GET: /Admin/Dashboard
    public IActionResult Dashboard()
    {
        return View();
    }

    // GET: /Admin/BlockedMobiles
    public async Task<IActionResult> BlockedMobiles()
    {
        var token = await _authService.GetAccessTokenAsync();
        var response = await _apiService.GetAsync<BlacklistResponse>("/api/admin/blacklist", token);

        if (!response.Success || response.Data == null)
        {
            TempData["ErrorMessage"] = response.Message ?? "Failed to load blocked mobiles";
            return View(new List<BlockedMobileResponse>());
        }

        var blockedMobiles = response.Data.BlockedMobiles
                .Select(mobile => new BlockedMobileResponse
                {
                    Mobile = mobile,
                    BlockedAt = DateTime.UtcNow,
                    BlockedBy = "Admin"
                })
                .ToList();

        return View(blockedMobiles ?? new List<BlockedMobileResponse>());
    }

    // POST: /Admin/BlockMobile
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BlockMobile(BlockMobileRequest model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Invalid mobile number";
            return RedirectToAction("BlockedMobiles");
        }

        var token = await _authService.GetAccessTokenAsync();
        var response = await _apiService.PostAsync<object>("/api/admin/blacklist/mobile", model, token);

        if (response.Success)
        {
            TempData["SuccessMessage"] = $"Mobile {model.Mobile} has been blocked successfully";
        }
        else
        {
            TempData["ErrorMessage"] = response.Message ?? "Failed to block mobile";
        }

        return RedirectToAction("BlockedMobiles");
    }

    // POST: /Admin/UnblockMobile
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UnblockMobile(string mobile)
    {
        if (string.IsNullOrWhiteSpace(mobile))
        {
            TempData["ErrorMessage"] = "Invalid mobile number";
            return RedirectToAction("BlockedMobiles");
        }

        var token = await _authService.GetAccessTokenAsync();
        
        // Use DELETE method via custom DELETE async method
        var response = await _apiService.DeleteAsync<object>($"/api/admin/blacklist/mobile/{mobile}", token);

        if (response.Success)
        {
            TempData["SuccessMessage"] = $"Mobile {mobile} has been unblocked successfully";
        }
        else
        {
            TempData["ErrorMessage"] = response.Message ?? "Failed to unblock mobile";
        }

        return RedirectToAction("BlockedMobiles");
    }

    // GET: /Admin/BlockedDomains
    public async Task<IActionResult> BlockedDomains()
    {
        var token = await _authService.GetAccessTokenAsync();
        var response = await _apiService.GetAsync<BlacklistResponse>("/api/admin/blacklist", token);

        if (!response.Success || response.Data == null)
        {
            TempData["ErrorMessage"] = response.Message ?? "Failed to load blocked domains";
            return View(new List<BlockedDomainResponse>());
        }

        var blockedDomainResponse = response.Data.BlockedDomains
        .Select(domain => new BlockedDomainResponse
        {
            Domain = domain,
            BlockedAt = DateTime.UtcNow,
            BlockedBy = "Admin"
        })
        .ToList();

        return View(blockedDomainResponse ?? new List<BlockedDomainResponse>());
    }

    // POST: /Admin/BlockDomain
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BlockDomain(BlockDomainRequest model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Invalid domain";
            return RedirectToAction("BlockedDomains");
        }

        var token = await _authService.GetAccessTokenAsync();
        var response = await _apiService.PostAsync<object>("/api/admin/blacklist/domain", model, token);

        if (response.Success)
        {
            TempData["SuccessMessage"] = $"Domain {model.Domain} has been blocked successfully";
        }
        else
        {
            TempData["ErrorMessage"] = response.Message ?? "Failed to block domain";
        }

        return RedirectToAction("BlockedDomains");
    }

    // POST: /Admin/UnblockDomain
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UnblockDomain(string domain)
    {
        if (string.IsNullOrWhiteSpace(domain))
        {
            TempData["ErrorMessage"] = "Invalid domain";
            return RedirectToAction("BlockedDomains");
        }

        var token = await _authService.GetAccessTokenAsync();
        
        // Use DELETE method via custom DELETE async method
        var response = await _apiService.DeleteAsync<object>($"/api/admin/blacklist/domain/{domain}", token);

        if (response.Success)
        {
            TempData["SuccessMessage"] = $"Domain {domain} has been unblocked successfully";
        }
        else
        {
            TempData["ErrorMessage"] = response.Message ?? "Failed to unblock domain";
        }

        return RedirectToAction("BlockedDomains");
    }

    // GET: /Admin/LoanApplications
    public async Task<IActionResult> LoanApplications(string? status = null)
    {
        var token = await _authService.GetAccessTokenAsync();
        var endpoint = string.IsNullOrWhiteSpace(status) 
            ? "/api/admin/loans" 
            : $"/api/admin/loans?status={status}";
        
        var response = await _apiService.GetAsync<List<LoanApplicationResponse>>(endpoint, token);

        if (!response.Success)
        {
            TempData["ErrorMessage"] = response.Message ?? "Failed to load loan applications";
            return View(new List<LoanApplicationResponse>());
        }

        ViewBag.CurrentStatus = status;
        return View(response.Data ?? new List<LoanApplicationResponse>());
    }

    // POST: /Admin/ApproveLoan
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveLoan(Guid id, string? notes = null)
    {
        var token = await _authService.GetAccessTokenAsync();
        var response = await _apiService.PostAsync<object>(
            $"/api/admin/loans/{id}/approve", 
            new { Notes = notes }, 
            token);

        if (response.Success)
        {
            TempData["SuccessMessage"] = "Loan application approved successfully";
        }
        else
        {
            TempData["ErrorMessage"] = response.Message ?? "Failed to approve loan application";
        }

        return RedirectToAction("LoanApplications");
    }

    // POST: /Admin/RejectLoan
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectLoan(Guid id, string? notes = null)
    {
        var token = await _authService.GetAccessTokenAsync();
        var response = await _apiService.PostAsync<object>(
            $"/api/admin/loans/{id}/reject", 
            new { Reason = notes }, 
            token);

        if (response.Success)
        {
            TempData["SuccessMessage"] = "Loan application rejected successfully";
        }
        else
        {
            TempData["ErrorMessage"] = response.Message ?? "Failed to reject loan application";
        }

        return RedirectToAction("LoanApplications");
    }
}
