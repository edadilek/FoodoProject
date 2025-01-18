using foodoProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace foodoProject.ViewModels
{
    public class ProfileViewModel
    {
        public User User { get; set; }
        public List<OrderViewModel> Orders { get; set; }

    }
}