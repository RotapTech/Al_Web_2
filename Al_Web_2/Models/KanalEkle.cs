//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Al_Web_2.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class KanalEkle
    {
        public KanalEkle()
        {
            this.SirketEkles = new HashSet<SirketEkle>();
            this.Kullanicilars = new HashSet<Kullanicilar>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public Nullable<bool> Silindi { get; set; }
    
        public virtual ICollection<SirketEkle> SirketEkles { get; set; }
        public virtual ICollection<Kullanicilar> Kullanicilars { get; set; }
    }
}
