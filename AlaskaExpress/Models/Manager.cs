//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AlaskaExpress.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Manager
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Manager()
        {
            this.Buses = new HashSet<Bus>();
            this.Sellers = new HashSet<Seller>();
        }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide Email")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Please provide valid Email")]
        [Display(Name = "Manager Email")]
        public string Manager_email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide Password")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Password should be min 5")]
        [Display(Name = "Password")]
        public string Manager_password { get; set; }

        [Display(Name = "Full Name")]
        public string Manager_fullname { get; set; }

        [Display(Name = "Address")]
        public string Manager_address { get; set; }

        [Display(Name = "NID Number")]
        public string Manager_nid { get; set; }

        [Display(Name = "Phone NUmber")]
        public string Manager_phone { get; set; }

        [Display(Name = "Manager Addedby")]
        public string Manager_addedby { get; set; }
    
        public virtual Admin Admin { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Bus> Buses { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Seller> Sellers { get; set; }
    }
}