//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DemirStore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public partial class tblAddress
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblAddress()
        {
            this.tblOrder = new HashSet<tblOrder>();
        }

        public int Id { get; set; }
        [Required(ErrorMessage = "Bu alan gerekli")]
        [DisplayName("Adres Ad�")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Bu alan gerekli")]
        [DisplayName("Adres")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Bu alan gerekli")]
        [DisplayName("�l")]
        public string City { get; set; }
        [Required(ErrorMessage = "Bu alan gerekli")]
        [DisplayName("�lke")]
        public string Country { get; set; }
        [Required(ErrorMessage = "Bu alan gerekli")]
        [DisplayName("Posta Kodu")]
        public Nullable<int> Zip { get; set; }
        public Nullable<int> UserId { get; set; }

        public virtual tblUsers tblUsers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblOrder> tblOrder { get; set; }
    }
}