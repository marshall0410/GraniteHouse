﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GraniteHouse.Models
{
    public class Products
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public bool Available { get; set; }

        public string Image { get; set; }

        public int ShadeColor { get; set; }

        [Display(Name = "Product Type")]
        public int ProductTypesId { get; set; }

        [ForeignKey("ProductTypesId")]
        public virtual ProductTypes ProductTypes {get; set;}

        [Display(Name ="Special Tag")]
        public int SpecialTagsId { get; set; }

        [ForeignKey("SpecialTagId")]
        public virtual SpecialTags SpecialTags { get; set; }
    }
}