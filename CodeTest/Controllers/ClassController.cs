using CodeTest.Model;
using CodeTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly BookingSystem _context;

        public ClassController(BookingSystem context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("insert")]
        public async Task<IActionResult> ClassInsertion([FromBody] Class classinfo)
        {
            var dbclass = _context.Class.Where(u => u.ClassId == classinfo.ClassId).FirstOrDefault();
            if (dbclass != null)
            {
                return BadRequest("Class already exists");
            }

            _context.Class.Add(classinfo);
            await _context.SaveChangesAsync();

            return Ok("Class is successfully created");
        }

        [HttpGet]
        [Route("available-classes")]
        public async Task<ActionResult<IEnumerable<Class>>> GetAvailableClasses()
        {
            var availableClasses = await _context.Class.Where(c => !c.IsFull).ToListAsync();
            return availableClasses;
        }

       
        [HttpPost]
        [Route("book-class")]
        public async Task<IActionResult> BookClass([FromBody] ClassBookingDTO bookingInfo)
        {

            var user = _context.UserInfo.FirstOrDefault(u => u.Id == bookingInfo.UserId);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            var selectedClass = await _context.Class.FirstOrDefaultAsync(c => c.ClassId == bookingInfo.ClassId);
            if (selectedClass == null)
            {
                return BadRequest("Class not found");
            }
            var currentDate = bookingInfo.CreatedDate;
            if (currentDate > selectedClass.StartTime || currentDate > selectedClass.EndTime)
            {
                user.Credit += selectedClass.RequiredCredit;
                return BadRequest("Can't book during overlap time");
            }
            user.Credit -= selectedClass.RequiredCredit;
            if (selectedClass.IsFull)
            {
               
                var waitlist = new Waitlist
                {
                    UserId = bookingInfo.UserId,
                    ClassId = bookingInfo.ClassId,
                    Email= bookingInfo.Email,
                };
                _context.WaitList.Add(waitlist);
                await _context.SaveChangesAsync();

              
              
                return Ok("Class is full. Please wait in WaitingList");
            }
            else
            {
                selectedClass.IsBook = true;

                selectedClass.TotalCurrentAcceptance++; 
                _context.Class.Update(selectedClass);

                if (selectedClass.TotalCurrentAcceptance >= selectedClass.TotalNoOfAcceptance)
                {
                    selectedClass.IsFull = true; 
                }

                var booking = new ClassBookingDTO
                {
                    UserId = bookingInfo.UserId,
                    ClassId = bookingInfo.ClassId,
                    Email = bookingInfo.Email,
                    IsBook = bookingInfo.IsBook,
                    CreatedDate = bookingInfo.CreatedDate
                };
                _context.BookingClass.Add(booking);
            }

            await _context.SaveChangesAsync();

            return Ok("Class booked successfully");
        }

        [HttpPost]
        [Route("cancel-class")]
        public async Task<IActionResult> CancelClass([FromBody] ClassBookingDTO bookingInfo)
        {
            var selectedClass = await _context.Class.FirstOrDefaultAsync(c => c.ClassId == bookingInfo.ClassId);
            if (selectedClass == null)
            {
                return BadRequest("Class not found");
            }

            var booking = await _context.BookingClass.FirstOrDefaultAsync(b => b.UserId == bookingInfo.UserId && b.ClassId == bookingInfo.ClassId);

            if (booking == null)
            {
                return BadRequest("User hasn't booked this class");
            }
            selectedClass.TotalCurrentAcceptance--;
            if (selectedClass.IsFull)
            {

                var waitlistUser = await _context.WaitList.Where(w => w.ClassId == bookingInfo.ClassId).OrderBy(w => w.WaitlistId).FirstOrDefaultAsync();
                if (waitlistUser != null)
                {
                    selectedClass.TotalCurrentAcceptance++; 
                    _context.Class.Update(selectedClass);
                    _context.WaitList.Remove(waitlistUser);

                    
                    selectedClass.IsBook = true;
                    _context.Class.Update(selectedClass);

                
                    var waitlistedBooking = new ClassBookingDTO
                    {
                        UserId = waitlistUser.UserId,
                        ClassId = waitlistUser.ClassId,
                        Email = waitlistUser.Email,
                        IsBook = true,
                        CreatedDate = DateTime.Now,
                    };
                    _context.BookingClass.Add(waitlistedBooking);
                }
                else
                {
                    selectedClass.IsBook = false;
                    selectedClass.IsFull = false; 
                    _context.Class.Update(selectedClass);
                }
            }
            else
            {
                selectedClass.IsBook = false;
            }

            _context.BookingClass.Remove(booking);
            await _context.SaveChangesAsync();

            return Ok("Class booking canceled");
        }

    
        [HttpPost]
        [Route("end-class")]
        public async Task<IActionResult> EndClass([FromBody] Class classInfo)
        {
            var endedClass = await _context.Class.FirstOrDefaultAsync(c => c.ClassId == classInfo.ClassId);
            if (endedClass == null)
            {
                return BadRequest("Class not found");
            }

            // Refund credits to waitlisted users
            var waitlistUsers = await _context.WaitList.Where(w => w.ClassId == classInfo.ClassId).ToListAsync();
            foreach (var waitlistUser in waitlistUsers)
            {
                var user = await _context.UserInfo.FirstOrDefaultAsync(u => u.Id == waitlistUser.UserId);
                if (user != null)
                {
                    user.Credit += 1; 

                }
            }

        

            endedClass.IsBook = false;
            _context.Class.Update(endedClass);

            await _context.SaveChangesAsync();

            return Ok("Class ended, waitlisted users refunded");
        }
    }
}


