﻿namespace Common.Constants
{
    public class RolesConstants
    {
        public const string AdministratorRoleName = "Administrator";
        public const string BarberRoleName = "Barber";
        public const string BarberShopOwnerRoleName = "Owner";
        public const string BarberShopOwnerOrAdmin = BarberShopOwnerRoleName + "," + AdministratorRoleName;
        public const string BarberShopOwnerOrBarber = BarberShopOwnerRoleName + "," + BarberRoleName;
    }
}
