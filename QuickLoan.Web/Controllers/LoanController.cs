using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using QuickLoan.Web.Interfaces;
using QuickLoan.Web.Models;

namespace QuickLoan.Web.Controllers;

public class LoanController : Controller
{
    private readonly ILoanService _loanService;
    private readonly IAuthService _authService;

    public LoanController(ILoanService loanService, IAuthService authService)
    {
        _loanService = loanService;
        _authService = authService;
    }

    // GET: /Loan/Apply - Quotation Calculator (Public)
    [HttpGet]
    public IActionResult Apply()
    {
        var model = new QuotationRequest
        {
            DateOfBirth = DateTime.Today.AddYears(-25)
        };
        return View(model);
    }

    // POST: /Loan/Apply - Calculate Quotation
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply(QuotationRequest model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var quotation = await _loanService.CalculateQuotationAsync(model);

        if (quotation.Success && quotation.Data != null)
        {
            TempData["Quotation"] = Newtonsoft.Json.JsonConvert.SerializeObject(quotation);
            TempData["QuotationRequest"] = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            return RedirectToAction("QuotationResult");
        }

        ModelState.AddModelError(string.Empty, quotation.Message);
        return View(model);
    }

    // GET: /Loan/QuotationResult - Show Quotation
    [HttpGet]
    public IActionResult QuotationResult()
    {
        if (TempData["Quotation"] == null)
        {
            return RedirectToAction("Apply");
        }

        // Keep TempData for next request (when redirecting to Submit)
        var quotationJson = TempData["Quotation"]?.ToString();
        var requestJson = TempData["QuotationRequest"]?.ToString();
        
        TempData.Keep("Quotation");
        TempData.Keep("QuotationRequest");

        var quotation = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResponse<QuotationResponse>>(quotationJson ?? "");
        var request = Newtonsoft.Json.JsonConvert.DeserializeObject<QuotationRequest>(requestJson ?? "");

        ViewBag.Request = request;
        return View(quotation.Data);
    }

    // GET: /Loan/Submit - Submit Application (Requires Login)
    [Authorize]
    [HttpGet]
    public IActionResult Submit()
    {
        if (TempData["QuotationRequest"] == null)
        {
            return RedirectToAction("Apply");
        }

        var requestJson = TempData["QuotationRequest"]?.ToString();
        TempData.Keep("QuotationRequest"); // Keep for POST
        
        var request = Newtonsoft.Json.JsonConvert.DeserializeObject<QuotationRequest>(requestJson ?? "");
        
        var model = new LoanApplicationRequest
        {
            AmountRequired = request?.AmountRequired ?? 0,
            Term = request?.Term ?? 0,
            ProductType = request?.ProductType ?? "ProductC",
            Title = request?.Title ?? "Mr",
            FirstName = request?.FirstName ?? "",
            LastName = request?.LastName ?? "",
            DateOfBirth = request.DateOfBirth,
            Mobile = request?.Mobile ?? "",
            Email = request?.Email ?? ""
        };

        return View(model);
    }

    // POST: /Loan/Submit - Submit Application
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(LoanApplicationRequest model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var loanId = await _loanService.SubmitLoanApplicationAsync(model);

        if (loanId != null)
        {
            TempData["SuccessMessage"] = "Your loan application has been submitted successfully!";
            return RedirectToAction("Details", new { id = loanId });
        }

        ModelState.AddModelError(string.Empty, "Failed to submit application. Please try again.");
        return View(model);
    }

    // GET: /Loan/Dashboard - User Dashboard
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var loans = await _loanService.GetMyLoansAsync();
        return View(loans ?? new List<LoanApplicationResponse>());
    }

    // GET: /Loan/Details/{id} - Loan Details
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var loan = await _loanService.GetLoanByIdAsync(id);

        if (loan == null)
        {
            TempData["ErrorMessage"] = "Loan application not found.";
            return RedirectToAction("Dashboard");
        }

        return View(loan);
    }

    // GET: /Loan/Edit/{id} - Edit Draft Loan
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var loan = await _loanService.GetLoanByIdAsync(id);

        if (loan == null)
        {
            TempData["ErrorMessage"] = "Loan application not found.";
            return RedirectToAction("Dashboard");
        }

        if (loan.Status != "Draft")
        {
            TempData["ErrorMessage"] = "Only draft applications can be edited.";
            return RedirectToAction("Details", new { id });
        }

        var model = new UpdateLoanRequest
        {
            AmountRequired = loan.AmountRequired,
            Term = loan.Term,
            ProductType = loan.ProductType
        };

        ViewBag.LoanId = id;
        return View(model);
    }

    // POST: /Loan/Edit/{id} - Update Draft Loan
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateLoanRequest model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.LoanId = id;
            return View(model);
        }

        var success = await _loanService.UpdateLoanAsync(id, model);

        if (success)
        {
            TempData["SuccessMessage"] = "Loan application updated successfully!";
            return RedirectToAction("Details", new { id });
        }

        ModelState.AddModelError(string.Empty, "Failed to update application.");
        ViewBag.LoanId = id;
        return View(model);
    }

    // GET: /Loan/View/{applicationUrl} - Public Loan View
    [HttpGet]
    public async Task<IActionResult> View(string applicationUrl)
    {
        var loan = await _loanService.GetLoanByUrlAsync(applicationUrl);

        if (loan == null)
        {
            TempData["ErrorMessage"] = "Loan application not found.";
            return RedirectToAction("Apply");
        }

        return View(loan);
    }
}
