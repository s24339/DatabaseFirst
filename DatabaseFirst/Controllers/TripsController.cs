using DatabaseFirst.Models;
using DatabaseFirst.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatabaseFirst.Controllers
{
    [Route("api/trips")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly TripContext _context;

        public TripsController(TripContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TripDTO>>> GetTrips()
        {
            var trips = await _context.Trips
                .Include(t => t.CountryTrips)
                .ThenInclude(ct => ct.IdCountryNavigation)
                .Include(t => t.ClientTrips)
                .ThenInclude(ct => ct.IdClientNavigation)
                .Select(t => new TripDTO
                {
                    Name = t.Name,
                    Description = t.Description,
                    DateFrom = t.DateFrom,
                    DateTo = t.DateTo,
                    MaxPeople = t.MaxPeople,
                    Countries = t.CountryTrips.Select(ct => new CountryDTO { Name = ct.IdCountryNavigation.Name }).ToList(),
                    Clients = t.ClientTrips.Select(ct => new ClientDTO { FirstName = ct.IdClientNavigation.FirstName, LastName = ct.IdClientNavigation.LastName }).ToList()
                })
                .ToListAsync();

            return Ok(trips);
        }

        [HttpPost("{idTrip}/clients")]
        public async Task<IActionResult> AssignClientToTrip(int idTrip, [FromBody] ClientDTO clientTripDto)
        {
            var trip = await _context.Trips.FindAsync(idTrip);
            if (trip == null)
            {
                return NotFound("Trip not found.");
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.Pesel == clientTripDto.Pesel);

            if (client == null)
            {
                client = new Client
                {
                    FirstName = clientTripDto.FirstName,
                    LastName = clientTripDto.LastName,
                    Email = clientTripDto.Email,
                    Telephone = clientTripDto.Telephone,
                    Pesel = clientTripDto.Pesel
                };
                _context.Clients.Add(client);
                await _context.SaveChangesAsync();
            }

            bool isAlreadyAssigned = await _context.ClientTrips
                .AnyAsync(ct => ct.IdClient == client.IdClient && ct.IdTrip == idTrip);

            if (isAlreadyAssigned)
            {
                return BadRequest("Client is already assigned to this trip.");
            }

            var clientTrip = new ClientTrip
            {
                IdClient = client.IdClient,
                IdTrip = idTrip,
                RegisteredAt = DateTime.Now,
                PaymentDate = clientTripDto.PaymentDate
            };

            _context.ClientTrips.Add(clientTrip);
            await _context.SaveChangesAsync();

            return Ok("Client assigned to trip successfully.");
        }
    }

}
