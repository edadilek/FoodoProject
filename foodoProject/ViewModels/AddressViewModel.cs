using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace foodoProject.ViewModels
{
    public class AddressViewModel
    {
        public string AddressName { get; set; }
        public string Street { get; set; }
        public string Neighbourhood { get; set; }
        public string District { get; set; }
        public int BuildingNo { get; set; }
        public int ApartmentNo { get; set; }
        public int CityID { get; set; }
        public IEnumerable<SelectListItem> Cities { get; set; } // For the dropdown list
    }
}
