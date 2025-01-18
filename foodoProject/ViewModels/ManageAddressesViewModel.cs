using foodoProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace foodoProject.ViewModels
{
    public class ManageAddressesViewModel
    {
        public Address NewAddress { get; set; }
        public IEnumerable<Address> Addresses { get; set; } // Eklediğimiz adresler için koleksiyon
        public IEnumerable<SelectListItem> Cities { get; set; }
    }

}