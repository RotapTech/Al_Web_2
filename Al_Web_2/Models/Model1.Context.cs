﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Al_WebEntities : DbContext
    {
        public Al_WebEntities()
            : base("name=Al_WebEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<sysdiagrams> sysdiagrams { get; set; }
        public DbSet<SirketEkle> SirketEkle { get; set; }
        public DbSet<Kullanicilar> Kullanicilars { get; set; }
        public DbSet<CihazEkle> CihazEkle { get; set; }
        public DbSet<KanalEkle> KanalEkles { get; set; }
    }
}
