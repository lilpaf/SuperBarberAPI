using Business.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Models.Responses.Barber
{
    //ToDo fix it when we create the FE
    public class AllBarbersResponse
    {
        public required IReadOnlyList<AllBarberDto> Barbers { get; init; }
    }
}
