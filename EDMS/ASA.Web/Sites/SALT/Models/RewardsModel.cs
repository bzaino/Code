﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASA.Web.Sites.SALT.Models
{
    public class RewardsModel
    {
        public string CouponCode { get; set; }
        public bool Eligible { get; set; }
        public string RewardLink { get; set; }

    }
}