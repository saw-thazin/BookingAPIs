using CodeTest.Model;
using CodeTest.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Identity;

namespace CodeTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly BookingSystem _context;

        public PackageController(BookingSystem context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("insert")]

        public async Task<IActionResult> PackageInsertion([FromBody] Package package)
        {
            var dbPackage = _context.Package.Where(u => u.PackageId == package.PackageId).FirstOrDefault();
            if (dbPackage != null)
            {
                return BadRequest("Package already exists");

            }

            _context.Package.Add(package);
            await _context.SaveChangesAsync();

            return Ok("Package is successfully created");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Package>>> GetAvailablePackages()
        {
            var availablePackages = await _context.Package
                .Where(u => u.Status == "Available")
                .ToListAsync();

            if (availablePackages != null && availablePackages.Any())
            {
                return availablePackages;
            }

            return NotFound("No available packages found.");
        }


        [HttpPost]
        [Route("buy-package")]
        public async Task<IActionResult> BuyPackage([FromBody] PackagePurchaseDTO packagePurchaseInfo)
        {
            var user = _context.UserInfo.FirstOrDefault(u => u.Email == packagePurchaseInfo.Email);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            var selectedPackage = _context.Package.FirstOrDefault(p => p.PackageId == packagePurchaseInfo.PackageId);
            if (selectedPackage == null)
            {
                return BadRequest("Package not found");
            }

            if (user.Credit < selectedPackage.RequiredCredit)
            {
                return BadRequest("Insufficient credit to buy this package");
            }

            user.Credit -= selectedPackage.RequiredCredit;
            

            _context.UserInfo.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Package purchased successfully", RemainingCredit = user.Credit , PurchasedPackageId = selectedPackage.PackageId});
        }

    }
}
