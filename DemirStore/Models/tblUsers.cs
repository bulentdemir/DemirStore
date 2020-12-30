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

    public partial class tblUsers
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblUsers()
        {
            this.tblAddress = new HashSet<tblAddress>();
            this.tblShoppingCart = new HashSet<tblShoppingCart>();
        }

        public int Id { get; set; }

        [DisplayName("�sim")]
        public string Name { get; set; }

        [DisplayName("Soyisim")]
        public string Surname { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail ge�erli de�il")]
        [Required(ErrorMessage = "E - mail ge�erli de�il")]
        [DisplayName("Mail Adresi")]
        public string Email { get; set; }

        [DisplayName("Parola")]
        [Required(ErrorMessage = "Bu alan gerekli")]
        [DataType(DataType.Password)]
        public string Pswd { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Parolay� Do�rula")]
        [Compare("Pswd")]
        public string ConfirmPswd { get; set; }

        public string PswdSalt { get; set; }

        [DisplayName("Telefon Numaras�")]
        public string PhoneNumber { get; set; }

        [DisplayName("Admin")]
        public Nullable<bool> isVerified { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblAddress> tblAddress { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblShoppingCart> tblShoppingCart { get; set; }
    }
}
