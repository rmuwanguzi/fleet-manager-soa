
using FleetManager.Shared.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace FleetManager.DbBase
{
    public class tableNames
    {
        //

       
        //
    }
    public class BaseContext : DbContext
    {
        bool _isServer { get; set; }
        string schema { get; set; }
        public BaseContext() { }
        
        public BaseContext(string _conn_string, string _schema_name, bool IsServer = true) :
            base(_conn_string)
        {
            this.schema = _schema_name;
            this._isServer = IsServer;
        }
        public BaseContext(DbConnection existingConnection, bool contextOwnConnection, string _schema_name, bool IsServer = true)
            : base(existingConnection, contextOwnConnection)
        {
            this.schema = _schema_name;
            this._isServer = IsServer;
        }

        
        public DbSet<vehicleC> VEHICLES { get; set; }
        public DbSet<vh_categoryC> VEHICLE_CATEGORIES { get; set; }
        public DbSet<vh_expense_typeC> EXPENSE_TYPES { get; set; }
        public DbSet<vh_driverC> DRIVERS { get; set; }
        public DbSet<vh_expense_type_fieldC> EXPENSE_TYPE_FIELDS { get; set; }
        public DbSet<vh_expense_type_field_d_itemC> EXPENSE_FIELD_TYPE_ITEMS { get; set; }
        public DbSet<vh_expense_transC> EXPENSE_TRANSACTIONS { get; set; }
        public DbSet<vh_expense_trans_fieldC> EXPENSE_TRANSACTIONS_FIELDS { get; set; }
        public DbSet<pc_userC> SYSTEM_USERS { get; set; }
        public DbSet<security_groupC> SECURITY_GROUPS_SET { get; set; }
        public DbSet<vh_expense_trans_voucherC> EXPENSE_TRANSACTIONS_VOUCHERS { get; set; }
        public DbSet<projectC> PROJECTS { get; set; }
        public DbSet<mechanicC> MECHANICS { get; set; }
        public DbSet<vehicle_ownerC> VEHICLE_OWNERS { get; set; }
        public DbSet<team_leaderC> TEAM_LEADERS { get; set; }
        public DbSet<fuel_stationC> FUEL_STATIONS { get; set; }
        public DbSet<assign_project_fuel_stationC> ASSIGN_PROJECT_FUEL_STATIONS { get; set; }
        public DbSet<creditorC> CREDITORS_SET { get; set; }
        public DbSet<creditor_depositC> CREDITOR_DEPOSITS { get; set; }
        public DbSet<creditor_invoiceC> CREDITOR_INVOICES { get; set; }
        public DbSet<creditor_invoice_paymentC> CREDITOR_INVOICE_PAYMENTS { get; set; }
        public DbSet<creditor_trans_statementC> CREDITOR_TRANS_STATMENTS { get; set; }
        private DatabaseGeneratedOption DbOptions
        {
            get
            {
                return _isServer ? DatabaseGeneratedOption.Identity : DatabaseGeneratedOption.None;
            }
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            if (!string.IsNullOrEmpty(schema))
            {
                modelBuilder.HasDefaultSchema(schema);
            }
                       //
            modelBuilder.Entity<vehicleC>().ToTable(tableNames.vehicle_tb);
            modelBuilder.Entity<vehicleC>().HasKey(e => new { e.vehicle_plate_no, e.delete_id });
            modelBuilder.Entity<vehicleC>().Property(e => e.vehicle_plate_no).HasColumnOrder(1);
            modelBuilder.Entity<vehicleC>().Property(e => e.delete_id).HasColumnOrder(2);
            modelBuilder.Entity<vehicleC>().Property(e => e.vehicle_id).HasDatabaseGeneratedOption(DbOptions);

            //
            modelBuilder.Entity<vh_categoryC>().ToTable(tableNames.vehicle_cat_tb);
            modelBuilder.Entity<vh_categoryC>().HasKey(e => new { e.delete_id,e.vehicle_cat_name });
            modelBuilder.Entity<vh_categoryC>().Property(e => e.vehicle_cat_name).HasColumnOrder(1);
            modelBuilder.Entity<vh_categoryC>().Property(e => e.delete_id).HasColumnOrder(2);
            modelBuilder.Entity<vh_categoryC>().Property<int>(e => e.vehicle_cat_id).HasDatabaseGeneratedOption(DbOptions);

             //
            modelBuilder.Entity<vh_driverC>().ToTable(tableNames.driver_tb);
            modelBuilder.Entity<vh_driverC>().HasKey(e => new { e.delete_id, e.driver_name });
            modelBuilder.Entity<vh_driverC>().Property(e => e.driver_name).HasColumnOrder(1);
            modelBuilder.Entity<vh_driverC>().Property(e => e.delete_id).HasColumnOrder(2);
            modelBuilder.Entity<vh_driverC>().Property<int>(e => e.driver_id).HasDatabaseGeneratedOption(DbOptions);
            //
            modelBuilder.Entity<vh_expense_typeC>().ToTable(tableNames.exp_type_tb);
            modelBuilder.Entity<vh_expense_typeC>().HasKey(e => new { e.delete_id, e.exp_type_name, e.exp_type_id });
            modelBuilder.Entity<vh_expense_typeC>().Property(e => e.exp_type_name).HasColumnOrder(1);
            modelBuilder.Entity<vh_expense_typeC>().Property(e => e.exp_type_id).HasColumnOrder(2);
            modelBuilder.Entity<vh_expense_typeC>().Property(e => e.delete_id).HasColumnOrder(3);
            modelBuilder.Entity<vh_expense_typeC>().Property<int>(e => e.exp_type_id).HasDatabaseGeneratedOption(DbOptions);
            //
            modelBuilder.Entity<vh_expense_type_fieldC>().ToTable(tableNames.exp_type_field_tb);
            modelBuilder.Entity<vh_expense_type_fieldC>().HasKey(e => new { e.delete_id, e.exp_type_id,e.et_field_name});
            modelBuilder.Entity<vh_expense_type_fieldC>().Property(e => e.et_field_name).HasColumnOrder(1);
            modelBuilder.Entity<vh_expense_type_fieldC>().Property(e => e.exp_type_id).HasColumnOrder(2);
            modelBuilder.Entity<vh_expense_type_fieldC>().Property(e => e.delete_id).HasColumnOrder(3);
            modelBuilder.Entity<vh_expense_type_fieldC>().Property<int>(e => e.et_field_id).HasDatabaseGeneratedOption(DbOptions);

            //
            modelBuilder.Entity<vh_expense_type_field_d_itemC>().ToTable(tableNames.exp_type_field_item_tb);
            modelBuilder.Entity<vh_expense_type_field_d_itemC>().HasKey(e => new { e.delete_id, e.et_field_item_name, e.exp_type_id, e.et_field_id});
            modelBuilder.Entity<vh_expense_type_field_d_itemC>().Property(e => e.et_field_item_name).HasColumnOrder(1);
            modelBuilder.Entity<vh_expense_type_field_d_itemC>().Property(e => e.exp_type_id).HasColumnOrder(2);
            modelBuilder.Entity<vh_expense_type_field_d_itemC>().Property(e => e.et_field_id).HasColumnOrder(3);
            modelBuilder.Entity<vh_expense_type_field_d_itemC>().Property(e => e.delete_id).HasColumnOrder(4);
            modelBuilder.Entity<vh_expense_type_field_d_itemC>().Property<int>(e => e.et_field_item_id).HasDatabaseGeneratedOption(DbOptions);
            //
            modelBuilder.Entity<vh_expense_transC>().ToTable(tableNames.expense_trans_tb);
            modelBuilder.Entity<vh_expense_transC>().HasKey(e => new { e.delete_id, e.expense_id });
            modelBuilder.Entity<vh_expense_transC>().Property(e => e.delete_id).HasColumnOrder(1);
            modelBuilder.Entity<vh_expense_transC>().Property(e => e.expense_id).HasColumnOrder(2);
            modelBuilder.Entity<vh_expense_transC>().Property<int>(e => e.expense_id).HasDatabaseGeneratedOption(DbOptions);
            //
            
            modelBuilder.Entity<pc_userC>().ToTable(tableNames.pc_user_tb);
            modelBuilder.Entity<pc_userC>().HasKey(e => new { e.delete_id, e.email_address });
            modelBuilder.Entity<pc_userC>().Property(e => e.email_address).HasColumnOrder(1);
            modelBuilder.Entity<pc_userC>().Property(e => e.delete_id).HasColumnOrder(2);
            modelBuilder.Entity<pc_userC>().Property<int>(e => e.user_id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            //

            modelBuilder.Entity<vh_expense_trans_fieldC>().ToTable(tableNames.expense_trans_field_tb);
            modelBuilder.Entity<vh_expense_trans_fieldC>().HasKey(e => new { e.delete_id, e.expense_id, e.et_field_id, e.et_field_item_id,e.exp_type_id });
            modelBuilder.Entity<vh_expense_trans_fieldC>().Property(e => e.delete_id).HasColumnOrder(1);
            modelBuilder.Entity<vh_expense_trans_fieldC>().Property(e => e.expense_id).HasColumnOrder(2);
            modelBuilder.Entity<vh_expense_trans_fieldC>().Property(e => e.et_field_id).HasColumnOrder(3);
            modelBuilder.Entity<vh_expense_trans_fieldC>().Property(e => e.et_field_item_id).HasColumnOrder(4);
            modelBuilder.Entity<vh_expense_trans_fieldC>().Property(e => e.exp_type_id).HasColumnOrder(5);
            modelBuilder.Entity<vh_expense_trans_fieldC>().Property<int>(e => e.un_id).HasDatabaseGeneratedOption(DbOptions);
            //
           
            //
            modelBuilder.Entity<vh_expense_trans_voucherC>().ToTable(tableNames.expense_trans_voucher_tb);
            modelBuilder.Entity<vh_expense_trans_voucherC>().HasKey(e => new { e.delete_id, e.voucher_no });
            modelBuilder.Entity<vh_expense_trans_voucherC>().Property(e => e.delete_id).HasColumnOrder(1);
            modelBuilder.Entity<vh_expense_trans_voucherC>().Property(e => e.voucher_no).HasColumnOrder(2);
            modelBuilder.Entity<vh_expense_trans_voucherC>().Property<int>(e => e.voucher_id).HasDatabaseGeneratedOption(DbOptions);
            //
            modelBuilder.Entity<mechanicC>().ToTable(tableNames.mechanic_tb);
            modelBuilder.Entity<mechanicC>().HasKey(e => new { e.mechanic_name, e.delete_id });
            modelBuilder.Entity<mechanicC>().Property(e => e.mechanic_name).HasColumnOrder(1);
            modelBuilder.Entity<mechanicC>().Property(e => e.delete_id).HasColumnOrder(2);
            modelBuilder.Entity<mechanicC>().Property(e => e.mechanic_id).HasDatabaseGeneratedOption(DbOptions);

            //
            modelBuilder.Entity<projectC>().ToTable(tableNames.project_tb);
            modelBuilder.Entity<projectC>().HasKey(e => new { e.project_name, e.delete_id });
            modelBuilder.Entity<projectC>().Property(e => e.project_name).HasColumnOrder(1);
            modelBuilder.Entity<projectC>().Property(e => e.delete_id).HasColumnOrder(2);
            modelBuilder.Entity<projectC>().Property(e => e.project_id).HasDatabaseGeneratedOption(DbOptions);

            //
            modelBuilder.Entity<vehicleC>().ToTable(tableNames.vehicle_tb);
            modelBuilder.Entity<vehicleC>().HasKey(e => new { e.vehicle_plate_no, e.delete_id });
            modelBuilder.Entity<vehicleC>().Property(e => e.vehicle_plate_no).HasColumnOrder(1);
            modelBuilder.Entity<vehicleC>().Property(e => e.delete_id).HasColumnOrder(2);
            modelBuilder.Entity<vehicleC>().Property(e => e.vehicle_id).HasDatabaseGeneratedOption(DbOptions);
            //
            modelBuilder.Entity<team_leaderC>().ToTable(tableNames.team_leader_tb);
            modelBuilder.Entity<team_leaderC>().HasKey(e => new { e.team_leader_name, e.delete_id });
            modelBuilder.Entity<team_leaderC>().Property(e => e.team_leader_name).HasColumnOrder(1);
            modelBuilder.Entity<team_leaderC>().Property(e => e.delete_id).HasColumnOrder(2);
            modelBuilder.Entity<team_leaderC>().Property(e => e.team_leader_id).HasDatabaseGeneratedOption(DbOptions);
            //
            modelBuilder.Entity<vehicle_ownerC>().ToTable(tableNames.vehicle_owner_tb);
            modelBuilder.Entity<vehicle_ownerC>().HasKey(e => new { e.vh_owner_name, e.delete_id });
            modelBuilder.Entity<vehicle_ownerC>().Property(e => e.vh_owner_name).HasColumnOrder(1);
            modelBuilder.Entity<vehicle_ownerC>().Property(e => e.delete_id).HasColumnOrder(2);
            modelBuilder.Entity<vehicle_ownerC>().Property(e => e.vh_owner_id).HasDatabaseGeneratedOption(DbOptions);
            //
            modelBuilder.Entity<fuel_stationC>().ToTable(tableNames.fuel_station_tb);
            modelBuilder.Entity<fuel_stationC>().HasKey(e => new { e.fuel_station_name, e.delete_id });
            modelBuilder.Entity<fuel_stationC>().Property(e => e.fuel_station_name).HasColumnOrder(1);
            modelBuilder.Entity<fuel_stationC>().Property(e => e.delete_id).HasColumnOrder(2);
            modelBuilder.Entity<fuel_stationC>().Property(e => e.fuel_station_id).HasDatabaseGeneratedOption(DbOptions);
            //
           
            //
            modelBuilder.Entity<creditorC>().ToTable(tableNames.creditor_tb);
            modelBuilder.Entity<creditorC>().HasKey(e => new { e.cr_account_type_id, e.cr_owner_id, e.delete_id });
            modelBuilder.Entity<creditorC>().Property(e => e.cr_account_type_id).HasColumnOrder(1);
            modelBuilder.Entity<creditorC>().Property(e => e.cr_owner_id).HasColumnOrder(2);
            modelBuilder.Entity<creditorC>().Property(e => e.delete_id).HasColumnOrder(3);
            modelBuilder.Entity<creditorC>().Property(e => e.cr_account_id).HasDatabaseGeneratedOption(DbOptions);
            //
            modelBuilder.Entity<creditor_depositC>().ToTable(tableNames.creditor_deposit_tb);
            modelBuilder.Entity<creditor_depositC>().HasKey(e => new { e.guid_key, e.cr_account_id, e.cr_payment_mode_id, e.delete_id });
            modelBuilder.Entity<creditor_depositC>().Property(e => e.guid_key).HasColumnOrder(1);
            modelBuilder.Entity<creditor_depositC>().Property(e => e.cr_account_id).HasColumnOrder(2);
            modelBuilder.Entity<creditor_depositC>().Property(e => e.delete_id).HasColumnOrder(3);
            modelBuilder.Entity<creditor_depositC>().Property(e => e.cr_deposit_id).HasDatabaseGeneratedOption(DbOptions);
            //
            modelBuilder.Entity<creditor_invoiceC>().ToTable(tableNames.creditor_invoice_tb);
            modelBuilder.Entity<creditor_invoiceC>().HasKey(e => new { e.expense_id, e.cr_account_id, e.delete_id });
            modelBuilder.Entity<creditor_invoiceC>().Property(e => e.expense_id).HasColumnOrder(1);
            modelBuilder.Entity<creditor_invoiceC>().Property(e => e.cr_account_id).HasColumnOrder(2);
            modelBuilder.Entity<creditor_invoiceC>().Property(e => e.delete_id).HasColumnOrder(3);
            modelBuilder.Entity<creditor_invoiceC>().Property(e => e.cr_invoice_id).HasDatabaseGeneratedOption(DbOptions);
            //
            modelBuilder.Entity<creditor_invoice_paymentC>().ToTable(tableNames.creditor_invoice_payment_tb);
            modelBuilder.Entity<creditor_invoice_paymentC>().HasKey(e => new { e.cr_pay_id, e.delete_id });
            modelBuilder.Entity<creditor_invoice_paymentC>().Property(e => e.cr_pay_id).HasColumnOrder(1);
            modelBuilder.Entity<creditor_invoice_paymentC>().Property(e => e.delete_id).HasColumnOrder(2);
            modelBuilder.Entity<creditor_invoice_paymentC>().Property(e => e.cr_pay_id).HasDatabaseGeneratedOption(DbOptions);
            //
            modelBuilder.Entity<creditor_trans_statementC>().ToTable(tableNames.creditor_trans_statement_tb);
            modelBuilder.Entity<creditor_trans_statementC>().HasKey(e => new { e.cr_account_id, e.cr_trans_source_id, e.cr_trans_source_type_id, e.delete_id });
   




        }


    }
    
}
