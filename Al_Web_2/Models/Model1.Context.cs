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
    
    public partial class Al_WebEntities1 : DbContext
    {
        public Al_WebEntities1()
            : base("name=Al_WebEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<CihazEkle> CihazEkle { get; set; }
        public DbSet<KanalEkle> KanalEkle { get; set; }
        public DbSet<Kullanicilar> Kullanicilar { get; set; }
        public DbSet<SirketEkle> SirketEkle { get; set; }
        public DbSet<sysdiagram> sysdiagram { get; set; }
    }
}
