﻿using Business.Interfaces;
using Business.Models.Exceptions;
using Business.Models.Requests.Barber;
using Business.Models.Responses.Barber;
using Common.Constants;
using Common.Constants.Messages;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Persistence.Entities;
using Persistence.Interfaces;

namespace Business.Implementations
{
    public class BarberService : IBarberService
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<BarberService> _logger;
        private readonly IUserHandler _userHandler;
        private readonly IBarberRepository _barberRepository;

        public BarberService(
            UserManager<User> userManager,
            ILogger<BarberService> logger,
            IUserHandler userHandler,
            IBarberRepository barberRepository)
        {
            _userManager = userManager;
            _logger = logger;
            _userHandler = userHandler;
            _barberRepository = barberRepository;
        }

        public async Task<RegisterBarberResponse> RegisterBarberAsync(RegisterBarberRequest request)
        {
            User user = await _userHandler.GetUserByUserClaimIdAsync();

            bool barberExists = await _barberRepository.BarberExistsByUserIdAsync(user.Id);

            if (barberExists)
            {
                _logger.LogError("User Id claim was not found");
                throw new InvalidArgumentException(Messages.UserIsAlreadyBarber);
            }

            Barber barber = new()
            {
                UserId = user.Id,
                About = request.About,
                IsDeleted = false,
                DeleteDate = null,
                AverageRating = 0
            };

            await _userManager.AddToRoleAsync(user, RolesConstants.BarberRoleName);

            Barber? deletedBarber = await _barberRepository.GetActiveOrDeletedBarberByUserIdAsync(user.Id);

            if (deletedBarber is not null)
            {
                deletedBarber.About = barber.About;
                deletedBarber.IsDeleted = barber.IsDeleted;
                deletedBarber.DeleteDate = barber.DeleteDate;
            }
            else
            {
                await _barberRepository.AddBarberAsync(barber);
            }

            await _barberRepository.SaveChangesAsync();

            return new RegisterBarberResponse
            {
                Message = Messages.BarberRegistrationSussesfuly
            };
        }
    }
}
