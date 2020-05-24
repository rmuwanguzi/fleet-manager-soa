using FleetManager.Shared.dto;
using FleetManager.Shared.Interfaces;
using FleetManager.Shared.Models;
using FleetManager.Shared.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace MutticoFleet.Server.Controllers
{
    //server code
    [Authorize]
    public class mfleetSysController : ApiController
    {
        string _controller_key;

        public mfleetSysController(IServiceFactory serviceFactory)
        {
            _ServiceFactory = serviceFactory;
            _controller_key = Guid.NewGuid().ToString();
            if (datam.DATA_CONTROLLER_MODEL_STATE.IndexOfKey(_controller_key) == -1)
            {
                datam.DATA_CONTROLLER_MODEL_STATE.Add(_controller_key, this.ModelState);
            }
        }
        public mfleetSysController()
        {
            _ServiceFactory = new ServiceFactory();
            _controller_key = Guid.NewGuid().ToString();
            if (datam.DATA_CONTROLLER_MODEL_STATE.IndexOfKey(_controller_key) == -1)
            {
                datam.DATA_CONTROLLER_MODEL_STATE.Add(_controller_key, this.ModelState);
            }
        }
        static IServiceFactory _ServiceFactory;



        #region ICheckForSoftwareUpdatesService
        //
        [HttpPost]
        [ActionName("CheckForUpdates")]
        [Route("CheckForUpdates")]
        [ResponseType(typeof(dto_check_software_update_responseC))]
        public IHttpActionResult _CheckForUpdates(dto_check_software_update_requestC dto)
        {
            var _service = _ServiceFactory.GetService<ICheckForSoftwareUpdatesService>();
            _service.controller_key = _controller_key;
            var _resp = _service.CheckForUpdates(dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }

        #endregion

        #region ICreditorService
        //
        [HttpGet]
        [ActionName("GetAllCreditors")]
        [Route("GetAllCreditors")]
        [ResponseType(typeof(List<creditorC>))]
        public IHttpActionResult _GetAllCreditors([FromUri] long fs_timestamp)
        {
            var _service = _ServiceFactory.GetService<ICreditorService>();
            _service.controller_key = _controller_key;
            var _resp = _service.GetAllCreditors(fs_timestamp).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpPost]
        [ActionName("EditCreditorOpeningCrBalance")]
        [Route("EditCreditorOpeningCrBalance")]
        [ResponseType(typeof(creditorC))]
        public IHttpActionResult _EditCreditorOpeningCrBalance(dto_creditor_update_opening_cr_balanceC _dto)
        {
            var _service = _ServiceFactory.GetService<ICreditorService>();
            _service.controller_key = _controller_key;
            var _resp = _service.EditCreditorOpeningCrBalance(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpPost]
        [ActionName("AddCreditorDeposit")]
        [Route("AddCreditorDeposit")]
        [ResponseType(typeof(creditor_depositC))]
        public IHttpActionResult _AddCreditorDeposit(dto_creditor_depositC _dto)
        {
            var _service = _ServiceFactory.GetService<ICreditorService>();
            _service.controller_key = _controller_key;
            var _resp = _service.AddCreditorDeposit(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("DeleteCreditorDeposit")]
        [Route("DeleteCreditorDeposit")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult _DeleteCreditorDeposit([FromUri] int cr_deposit_id)
        {
            var _service = _ServiceFactory.GetService<ICreditorService>();
            _service.controller_key = _controller_key;
            var _resp = _service.DeleteCreditorDeposit(cr_deposit_id).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpPost]
        [ActionName("GetAllCreditorDepositsByRange")]
        [Route("GetAllCreditorDepositsByRange")]
        [ResponseType(typeof(List<creditor_depositC>))]
        public IHttpActionResult _GetAllCreditorDepositsByRange(dto_fs_rangeC _dto)
        {
            var _service = _ServiceFactory.GetService<ICreditorService>();
            _service.controller_key = _controller_key;
            var _resp = _service.GetAllCreditorDepositsByRange(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpPost]
        [ActionName("GetAllCreditorDepositsByMonthPartition")]
        [Route("GetAllCreditorDepositsByMonthPartition")]
        [ResponseType(typeof(List<creditor_depositC>))]
        public IHttpActionResult _GetAllCreditorDepositsByMonthPartition(dto_month_partitionC _dto)
        {
            var _service = _ServiceFactory.GetService<ICreditorService>();
            _service.controller_key = _controller_key;
            var _resp = _service.GetAllCreditorDepositsByMonthPartition(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("GetCreditorDepositsByCreditorId")]
        [Route("GetCreditorDepositsByCreditorId")]
        [ResponseType(typeof(List<creditor_depositC>))]
        public IHttpActionResult _GetCreditorDepositsByCreditorId([FromUri] int cr_account_id, long fs_timestamp)
        {
            var _service = _ServiceFactory.GetService<ICreditorService>();
            _service.controller_key = _controller_key;
            var _resp = _service.GetCreditorDepositsByCreditorId(cr_account_id, fs_timestamp).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("GetCreditorSingle")]
        [Route("GetCreditorSingle")]
        [ResponseType(typeof(creditorC))]
        public IHttpActionResult _GetCreditorSingle([FromUri] int cr_account_id)
        {
            var _service = _ServiceFactory.GetService<ICreditorService>();
            _service.controller_key = _controller_key;
            var _resp = _service.GetCreditorSingle(cr_account_id).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }

        #endregion

        #region IFuelStationService
        //
        [HttpPost]
        [ActionName("AddFuelStation")]
        [Route("AddFuelStation")]
        [ResponseType(typeof(fuel_stationC))]
        public IHttpActionResult _AddFuelStation(dto_fuel_station_newC _dto)
        {
            var _service = _ServiceFactory.GetService<IFuelStationService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Add(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("GetAllFuelStationsByFsTimeStamp")]
        [Route("GetAllFuelStationsByFsTimeStamp")]
        [ResponseType(typeof(List<fuel_stationC>))]
        public IHttpActionResult _GetAllFuelStationsByFsTimeStamp([FromUri] long fs_timestamp)
        {
            var _service = _ServiceFactory.GetService<IFuelStationService>();
            _service.controller_key = _controller_key;
            var _resp = _service.GetAll(fs_timestamp).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpPost]
        [ActionName("UpdateFuelStation")]
        [Route("UpdateFuelStation")]
        [ResponseType(typeof(fuel_stationC))]
        public IHttpActionResult _UpdateFuelStation(dto_fuel_station_updateC _dto)
        {
            var _service = _ServiceFactory.GetService<IFuelStationService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Update(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("DeleteFuelStation")]
        [Route("DeleteFuelStation")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult _DeleteFuelStation([FromUri] int id)
        {
            var _service = _ServiceFactory.GetService<IFuelStationService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Delete(id).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }

        #endregion

        #region IDriverService
        //
        [HttpPost]
        [ActionName("AddDriver")]
        [Route("AddDriver")]
        [ResponseType(typeof(vh_driverC))]
        public IHttpActionResult _AddDriver(dto_driver_newC _dto)
        {
            var _service = _ServiceFactory.GetService<IDriverService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Add(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("GetAllDriversByFsTimeStamp")]
        [Route("GetAllDriversByFsTimeStamp")]
        [ResponseType(typeof(List<vh_driverC>))]
        public IHttpActionResult _GetAllDriversByFsTimeStamp([FromUri] long fs_timestamp)
        {
            var _service = _ServiceFactory.GetService<IDriverService>();
            _service.controller_key = _controller_key;
            var _resp = _service.GetAll(fs_timestamp).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpPost]
        [ActionName("UpdateDriver")]
        [Route("UpdateDriver")]
        [ResponseType(typeof(vh_driverC))]
        public IHttpActionResult _UpdateDriver(dto_driver_updateC _dto)
        {
            var _service = _ServiceFactory.GetService<IDriverService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Update(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("DeleteDriver")]
        [Route("DeleteDriver")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult _DeleteDriver([FromUri] int id)
        {
            var _service = _ServiceFactory.GetService<IDriverService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Delete(id).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }

        #endregion

        #region IExpenseService
        //
        [HttpPost]
        [ActionName("AddExpense")]
        [Route("AddExpense")]
        [ResponseType(typeof(List<vh_expense_transC>))]
        public IHttpActionResult _AddExpense(dto_expense_trans_voucher_newC _dto)
        {
            var _service = _ServiceFactory.GetService<IExpenseService>();
            _service.controller_key = _controller_key;
            var _resp = _service.AddExpense(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpPost]
        [ActionName("GetAllExpensesRange")]
        [Route("GetAllExpensesRange")]
        [ResponseType(typeof(List<vh_expense_transC>))]
        public IHttpActionResult _GetAllExpensesRange(dto_fs_rangeC _dto)
        {
            var _service = _ServiceFactory.GetService<IExpenseService>();
            _service.controller_key = _controller_key;
            var _resp = _service.GetAllExpensesRange(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpPost]
        [ActionName("GetAllExpensesByMonthPartition")]
        [Route("GetAllExpensesByMonthPartition")]
        [ResponseType(typeof(List<vh_expense_transC>))]
        public IHttpActionResult _GetAllExpensesByMonthPartition(dto_month_partitionC _dto)
        {
            var _service = _ServiceFactory.GetService<IExpenseService>();
            _service.controller_key = _controller_key;
            var _resp = _service.GetAllExpensesByMonthPartition(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpPost]
        [ActionName("GetAllRecentVehicleExpenses")]
        [Route("GetAllRecentVehicleExpenses")]
        [ResponseType(typeof(List<vh_expense_transC>))]
        public IHttpActionResult _GetAllRecentVehicleExpenses(dto_vehicle_expense_recentC _dto)
        {
            var _service = _ServiceFactory.GetService<IExpenseService>();
            _service.controller_key = _controller_key;
            var _resp = _service.GetAllRecentVehicleExpenses(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("DeleteExpenseVoucher")]
        [Route("DeleteExpenseVoucher")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult _DeleteExpenseVoucher([FromUri] int voucher_id)
        {
            var _service = _ServiceFactory.GetService<IExpenseService>();
            _service.controller_key = _controller_key;
            var _resp = _service.DeleteExpenseVoucher(voucher_id).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("GetAllExpensesByFsTimeStamp")]
        [Route("GetAllExpensesByFsTimeStamp")]
        [ResponseType(typeof(List<vh_expense_transC>))]
        public IHttpActionResult _GetAllExpensesByFsTimeStamp([FromUri] long fs_timestamp)
        {
            var _service = _ServiceFactory.GetService<IExpenseService>();
            _service.controller_key = _controller_key;
            var _resp = _service.GetAll(fs_timestamp).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("GetSingleExpense")]
        [Route("GetSingleExpense")]
        [ResponseType(typeof(vh_expense_transC))]
        public IHttpActionResult _GetSingleExpense([FromUri] int id)
        {
            var _service = _ServiceFactory.GetService<IExpenseService>();
            _service.controller_key = _controller_key;
            var _resp = _service.GetSingle(id).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("DeleteExpense")]
        [Route("DeleteExpense")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult _DeleteExpense([FromUri] int id)
        {
            var _service = _ServiceFactory.GetService<IExpenseService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Delete(id).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }

        #endregion

        #region IExpenseTypeFieldItemService
        //
        [HttpPost]
        [ActionName("AddExpenseTypeFieldItem")]
        [Route("AddExpenseTypeFieldItem")]
        [ResponseType(typeof(vh_expense_type_field_d_itemC))]
        public IHttpActionResult _AddExpenseTypeFieldItem(dto_expense_type_field_item_newC _dto)
        {
            var _service = _ServiceFactory.GetService<IExpenseTypeFieldItemService>();
            _service.controller_key = _controller_key;
            var _resp = _service.AddExpenseTypeFieldItem(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpPost]
        [ActionName("UpdateExpenseTypeFieldItem")]
        [Route("UpdateExpenseTypeFieldItem")]
        [ResponseType(typeof(vh_expense_type_field_d_itemC))]
        public IHttpActionResult _UpdateExpenseTypeFieldItem(dto_expense_type_field_item_updateC _dto)
        {
            var _service = _ServiceFactory.GetService<IExpenseTypeFieldItemService>();
            _service.controller_key = _controller_key;
            var _resp = _service.UpdateExpenseTypeFieldItem(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("GetAllExpenseTypeFieldItemsByFsTimeStamp")]
        [Route("GetAllExpenseTypeFieldItemsByFsTimeStamp")]
        [ResponseType(typeof(List<vh_expense_type_field_d_itemC>))]
        public IHttpActionResult _GetAllExpenseTypeFieldItemsByFsTimeStamp([FromUri] long fs_timestamp)
        {
            var _service = _ServiceFactory.GetService<IExpenseTypeFieldItemService>();
            _service.controller_key = _controller_key;
            var _resp = _service.GetAll(fs_timestamp).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("GetSingleExpenseTypeFieldItem")]
        [Route("GetSingleExpenseTypeFieldItem")]
        [ResponseType(typeof(vh_expense_type_field_d_itemC))]
        public IHttpActionResult _GetSingleExpenseTypeFieldItem([FromUri] int id)
        {
            var _service = _ServiceFactory.GetService<IExpenseTypeFieldItemService>();
            _service.controller_key = _controller_key;
            var _resp = _service.GetSingle(id).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("DeleteExpenseTypeFieldItem")]
        [Route("DeleteExpenseTypeFieldItem")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult _DeleteExpenseTypeFieldItem([FromUri] int id)
        {
            var _service = _ServiceFactory.GetService<IExpenseTypeFieldItemService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Delete(id).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }

        #endregion

        #region IExpenseTypeFieldService
        //
        [HttpPost]
        [ActionName("AddExpenseTypeField")]
        [Route("AddExpenseTypeField")]
        [ResponseType(typeof(vh_expense_type_fieldC))]
        public IHttpActionResult _AddExpenseTypeField(dto_expense_type_field_new _dto)
        {
            var _service = _ServiceFactory.GetService<IExpenseTypeFieldService>();
            _service.controller_key = _controller_key;
            var _resp = _service.AddExpenseTypeField(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpPost]
        [ActionName("UpdateExpenseTypeField")]
        [Route("UpdateExpenseTypeField")]
        [ResponseType(typeof(vh_expense_type_fieldC))]
        public IHttpActionResult _UpdateExpenseTypeField(dto_expense_type_field_updateC dto)
        {
            var _service = _ServiceFactory.GetService<IExpenseTypeFieldService>();
            _service.controller_key = _controller_key;
            var _resp = _service.UpdateExpenseTypeField(dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("GetAllExpenseTypeFieldsByFsTimeStamp")]
        [Route("GetAllExpenseTypeFieldsByFsTimeStamp")]
        [ResponseType(typeof(List<vh_expense_type_fieldC>))]
        public IHttpActionResult _GetAllExpenseTypeFieldsByFsTimeStamp([FromUri] long fs_timestamp)
        {
            var _service = _ServiceFactory.GetService<IExpenseTypeFieldService>();
            _service.controller_key = _controller_key;
            var _resp = _service.GetAll(fs_timestamp).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("GetSingleExpenseTypeField")]
        [Route("GetSingleExpenseTypeField")]
        [ResponseType(typeof(vh_expense_type_fieldC))]
        public IHttpActionResult _GetSingleExpenseTypeField([FromUri] int id)
        {
            var _service = _ServiceFactory.GetService<IExpenseTypeFieldService>();
            _service.controller_key = _controller_key;
            var _resp = _service.GetSingle(id).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("DeleteExpenseTypeField")]
        [Route("DeleteExpenseTypeField")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult _DeleteExpenseTypeField([FromUri] int id)
        {
            var _service = _ServiceFactory.GetService<IExpenseTypeFieldService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Delete(id).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }

        #endregion

        #region IExpenseTypeService
        //
        [HttpPost]
        [ActionName("AddExpenseType")]
        [Route("AddExpenseType")]
        [ResponseType(typeof(vh_expense_typeC))]
        public IHttpActionResult _AddExpenseType(dto_expense_type_newC _dto)
        {
            var _service = _ServiceFactory.GetService<IExpenseTypeService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Add(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("GetAllExpenseTypesByFsTimeStamp")]
        [Route("GetAllExpenseTypesByFsTimeStamp")]
        [ResponseType(typeof(List<vh_expense_typeC>))]
        public IHttpActionResult _GetAllExpenseTypesByFsTimeStamp([FromUri] long fs_timestamp)
        {
            var _service = _ServiceFactory.GetService<IExpenseTypeService>();
            _service.controller_key = _controller_key;
            var _resp = _service.GetAll(fs_timestamp).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpPost]
        [ActionName("UpdateExpenseType")]
        [Route("UpdateExpenseType")]
        [ResponseType(typeof(vh_expense_typeC))]
        public IHttpActionResult _UpdateExpenseType(dto_expense_type_updateC _dto)
        {
            var _service = _ServiceFactory.GetService<IExpenseTypeService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Update(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("DeleteExpenseType")]
        [Route("DeleteExpenseType")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult _DeleteExpenseType([FromUri] int id)
        {
            var _service = _ServiceFactory.GetService<IExpenseTypeService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Delete(id).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }

        #endregion
               
        #region IMechanicService
        //
        [HttpPost]
        [ActionName("AddMechanic")]
        [Route("AddMechanic")]
        [ResponseType(typeof(mechanicC))]
        public IHttpActionResult _AddMechanic(dto_mechanic_newC _dto)
        {
            var _service = _ServiceFactory.GetService<IMechanicService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Add(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("GetAllMechanicsByFsTimeStamp")]
        [Route("GetAllMechanicsByFsTimeStamp")]
        [ResponseType(typeof(List<mechanicC>))]
        public IHttpActionResult _GetAllMechanicsByFsTimeStamp([FromUri] long fs_timestamp)
        {
            var _service = _ServiceFactory.GetService<IMechanicService>();
            _service.controller_key = _controller_key;
            var _resp = _service.GetAll(fs_timestamp).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpPost]
        [ActionName("UpdateMechanic")]
        [Route("UpdateMechanic")]
        [ResponseType(typeof(mechanicC))]
        public IHttpActionResult _UpdateMechanic(dto_mechanic_updateC _dto)
        {
            var _service = _ServiceFactory.GetService<IMechanicService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Update(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("DeleteMechanic")]
        [Route("DeleteMechanic")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult _DeleteMechanic([FromUri] int id)
        {
            var _service = _ServiceFactory.GetService<IMechanicService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Delete(id).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }

        #endregion

        #region IInitDataService
        //
        [HttpGet]
        [ActionName("GetLoggedInUserInitData")]
        [Route("GetLoggedInUserInitData")]
        [ResponseType(typeof(dto_init_dataC))]
        public IHttpActionResult _GetLoggedInUserInitData()
        {
            var _service = _ServiceFactory.GetService<IInitDataService>();
            _service.controller_key = _controller_key;
            var _resp = _service.GetLoggedInUserInitData().Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }

        #endregion

        #region IRefreshTokenService
        //
        [HttpGet]
        [ActionName("GetRefreshToken")]
        [Route("GetRefreshToken")]
        [ResponseType(typeof(dto_refresh_tokenC))]
        public IHttpActionResult _GetRefreshToken()
        {
            var _service = _ServiceFactory.GetService<IRefreshTokenService>();
            _service.controller_key = _controller_key;
            var _resp = _service.GetRefreshToken().Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }

        #endregion

        #region IResetDbService
        //
        [HttpPost]
        [ActionName("ResetDb")]
        [Route("ResetDb")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult _ResetDb()
        {
            var _service = _ServiceFactory.GetService<IResetDbService>();
            _service.controller_key = _controller_key;
            var _resp = _service.ResetDb().Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }

        #endregion

        #region ISecurityGroupService
        //
        [HttpPost]
        [ActionName("AssignRightsToSecurityGroup")]
        [Route("AssignRightsToSecurityGroup")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult _AssignRightsToSecurityGroup(dto_security_group_assign_rightsC dto)
        {
            var _service = _ServiceFactory.GetService<ISecurityGroupService>();
            _service.controller_key = _controller_key;
            var _resp = _service.AssignRightsToSecurityGroup(dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpPost]
        [ActionName("AssignUserToSecurityGroup")]
        [Route("AssignUserToSecurityGroup")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult _AssignUserToSecurityGroup(dto_security_group_assign_userC dto)
        {
            var _service = _ServiceFactory.GetService<ISecurityGroupService>();
            _service.controller_key = _controller_key;
            var _resp = _service.AssignUserToSecurityGroup(dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpPost]
        [ActionName("RemoveUserFromSecurityGroup")]
        [Route("RemoveUserFromSecurityGroup")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult _RemoveUserFromSecurityGroup(dto_security_group_assign_userC dto)
        {
            var _service = _ServiceFactory.GetService<ISecurityGroupService>();
            _service.controller_key = _controller_key;
            var _resp = _service.RemoveUserFromSecurityGroup(dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("FetchSecurityGroupRightsFromServer")]
        [Route("FetchSecurityGroupRightsFromServer")]
        [ResponseType(typeof(string))]
        public IHttpActionResult _FetchSecurityGroupRightsFromServer([FromUri] int sec_group_id)
        {
            var _service = _ServiceFactory.GetService<ISecurityGroupService>();
            _service.controller_key = _controller_key;
            var _resp = _service.FetchSecurityGroupRightsFromServer(sec_group_id).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpPost]
        [ActionName("AddSecurityGroup")]
        [Route("AddSecurityGroup")]
        [ResponseType(typeof(security_groupC))]
        public IHttpActionResult _AddSecurityGroup(dto_security_group_newC _dto)
        {
            var _service = _ServiceFactory.GetService<ISecurityGroupService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Add(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("GetAllSecurityGroupsByFsTimeStamp")]
        [Route("GetAllSecurityGroupsByFsTimeStamp")]
        [ResponseType(typeof(List<security_groupC>))]
        public IHttpActionResult _GetAllSecurityGroupsByFsTimeStamp([FromUri] long fs_timestamp)
        {
            var _service = _ServiceFactory.GetService<ISecurityGroupService>();
            _service.controller_key = _controller_key;
            var _resp = _service.GetAll(fs_timestamp).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpPost]
        [ActionName("UpdateSecurityGroup")]
        [Route("UpdateSecurityGroup")]
        [ResponseType(typeof(security_groupC))]
        public IHttpActionResult _UpdateSecurityGroup(dto_security_group_updateC _dto)
        {
            var _service = _ServiceFactory.GetService<ISecurityGroupService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Update(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("DeleteSecurityGroup")]
        [Route("DeleteSecurityGroup")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult _DeleteSecurityGroup([FromUri] int id)
        {
            var _service = _ServiceFactory.GetService<ISecurityGroupService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Delete(id).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }

        #endregion

        #region IProjectService
        //
        [HttpPost]
        [ActionName("AssignFuelStationToProject")]
        [Route("AssignFuelStationToProject")]
        [ResponseType(typeof(assign_project_fuel_stationC))]
        public IHttpActionResult _AssignFuelStationToProject(dto_project_assign_fuelstationC dto)
        {
            var _service = _ServiceFactory.GetService<IProjectService>();
            _service.controller_key = _controller_key;
            var _resp = _service.AssignFuelStationToProject(dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpPost]
        [ActionName("RemoveFuelStationFromProject")]
        [Route("RemoveFuelStationFromProject")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult _RemoveFuelStationFromProject(dto_project_remove_fuelstationC dto)
        {
            var _service = _ServiceFactory.GetService<IProjectService>();
            _service.controller_key = _controller_key;
            var _resp = _service.RemoveFuelStationFromProject(dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("GetFuelStationsAssignedToProjects")]
        [Route("GetFuelStationsAssignedToProjects")]
        [ResponseType(typeof(List<assign_project_fuel_stationC>))]
        public IHttpActionResult _GetFuelStationsAssignedToProjects([FromUri] long fs_timestamp)
        {
            var _service = _ServiceFactory.GetService<IProjectService>();
            _service.controller_key = _controller_key;
            var _resp = _service.GetFuelStationsAssignedToProjects(fs_timestamp).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpPost]
        [ActionName("AddProject")]
        [Route("AddProject")]
        [ResponseType(typeof(projectC))]
        public IHttpActionResult _AddProject(dto_project_newC _dto)
        {
            var _service = _ServiceFactory.GetService<IProjectService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Add(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("GetAllProjectsByFsTimeStamp")]
        [Route("GetAllProjectsByFsTimeStamp")]
        [ResponseType(typeof(List<projectC>))]
        public IHttpActionResult _GetAllProjectsByFsTimeStamp([FromUri] long fs_timestamp)
        {
            var _service = _ServiceFactory.GetService<IProjectService>();
            _service.controller_key = _controller_key;
            var _resp = _service.GetAll(fs_timestamp).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpPost]
        [ActionName("UpdateProject")]
        [Route("UpdateProject")]
        [ResponseType(typeof(projectC))]
        public IHttpActionResult _UpdateProject(dto_project_updateC _dto)
        {
            var _service = _ServiceFactory.GetService<IProjectService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Update(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("DeleteProject")]
        [Route("DeleteProject")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult _DeleteProject([FromUri] int id)
        {
            var _service = _ServiceFactory.GetService<IProjectService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Delete(id).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }

        #endregion

        #region ITeamLeaderService
        //
        [HttpPost]
        [ActionName("AddTeamLeader")]
        [Route("AddTeamLeader")]
        [ResponseType(typeof(team_leaderC))]
        public IHttpActionResult _AddTeamLeader(dto_team_leader_newC _dto)
        {
            var _service = _ServiceFactory.GetService<ITeamLeaderService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Add(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("GetAllTeamLeadersByFsTimeStamp")]
        [Route("GetAllTeamLeadersByFsTimeStamp")]
        [ResponseType(typeof(List<team_leaderC>))]
        public IHttpActionResult _GetAllTeamLeadersByFsTimeStamp([FromUri] long fs_timestamp)
        {
            var _service = _ServiceFactory.GetService<ITeamLeaderService>();
            _service.controller_key = _controller_key;
            var _resp = _service.GetAll(fs_timestamp).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpPost]
        [ActionName("UpdateTeamLeader")]
        [Route("UpdateTeamLeader")]
        [ResponseType(typeof(team_leaderC))]
        public IHttpActionResult _UpdateTeamLeader(dto_team_leader_updateC _dto)
        {
            var _service = _ServiceFactory.GetService<ITeamLeaderService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Update(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("DeleteTeamLeader")]
        [Route("DeleteTeamLeader")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult _DeleteTeamLeader([FromUri] int id)
        {
            var _service = _ServiceFactory.GetService<ITeamLeaderService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Delete(id).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }

        #endregion

        #region IUserService
        //
        [HttpPost]
        [ActionName("AddNewUser")]
        [Route("AddNewUser")]
        [ResponseType(typeof(dto_pc_userC))]
        public IHttpActionResult _AddNewUser(dto_pc_user_newC _dto)
        {
            var _service = _ServiceFactory.GetService<IUserService>();
            _service.controller_key = _controller_key;
            var _resp = _service.AddNewUser(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("GetAllUsers")]
        [Route("GetAllUsers")]
        [ResponseType(typeof(List<dto_pc_userC>))]
        public IHttpActionResult _GetAllUsers([FromUri] long fs_timestamp)
        {
            var _service = _ServiceFactory.GetService<IUserService>();
            _service.controller_key = _controller_key;
            var _resp = _service.GetAllUsers(fs_timestamp).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("DeleteUser")]
        [Route("DeleteUser")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult _DeleteUser([FromUri] int id)
        {
            var _service = _ServiceFactory.GetService<IUserService>();
            _service.controller_key = _controller_key;
            var _resp = _service.DeleteUser(id).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpPost]
        [ActionName("UpdateUser")]
        [Route("UpdateUser")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult _UpdateUser(dto_pc_user_updateC _dto)
        {
            var _service = _ServiceFactory.GetService<IUserService>();
            _service.controller_key = _controller_key;
            var _resp = _service.UpdateUser(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpPost]
        [ActionName("ChangeUserPwd")]
        [Route("ChangeUserPwd")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult _ChangeUserPwd(dto_pc_user_change_password _dto)
        {
            var _service = _ServiceFactory.GetService<IUserService>();
            _service.controller_key = _controller_key;
            var _resp = _service.ChangeUserPwd(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("ResetPassword")]
        [Route("ResetPassword")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult _ResetPassword([FromUri] int user_id)
        {
            var _service = _ServiceFactory.GetService<IUserService>();
            _service.controller_key = _controller_key;
            var _resp = _service.ResetPassword(user_id).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }

        #endregion

        #region IVehicleCatService
        //
        [HttpPost]
        [ActionName("AddVehicleCategory")]
        [Route("AddVehicleCategory")]
        [ResponseType(typeof(vh_categoryC))]
        public IHttpActionResult _AddVehicleCategory(dto_vehicle_category_newC dto)
        {
            var _service = _ServiceFactory.GetService<IVehicleCatService>();
            _service.controller_key = _controller_key;
            var _resp = _service.AddVehicleCategory(dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpPost]
        [ActionName("UpdateVehicleCategory")]
        [Route("UpdateVehicleCategory")]
        [ResponseType(typeof(vh_categoryC))]
        public IHttpActionResult _UpdateVehicleCategory(dto_vehicle_category_updateC dto)
        {
            var _service = _ServiceFactory.GetService<IVehicleCatService>();
            _service.controller_key = _controller_key;
            var _resp = _service.UpdateVehicleCategory(dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("GetAllVehicleCatsByFsTimeStamp")]
        [Route("GetAllVehicleCatsByFsTimeStamp")]
        [ResponseType(typeof(List<vh_categoryC>))]
        public IHttpActionResult _GetAllVehicleCatsByFsTimeStamp([FromUri] long fs_timestamp)
        {
            var _service = _ServiceFactory.GetService<IVehicleCatService>();
            _service.controller_key = _controller_key;
            var _resp = _service.GetAll(fs_timestamp).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("GetSingleVehicleCat")]
        [Route("GetSingleVehicleCat")]
        [ResponseType(typeof(vh_categoryC))]
        public IHttpActionResult _GetSingleVehicleCat([FromUri] int id)
        {
            var _service = _ServiceFactory.GetService<IVehicleCatService>();
            _service.controller_key = _controller_key;
            var _resp = _service.GetSingle(id).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("DeleteVehicleCat")]
        [Route("DeleteVehicleCat")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult _DeleteVehicleCat([FromUri] int id)
        {
            var _service = _ServiceFactory.GetService<IVehicleCatService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Delete(id).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }

        #endregion

        #region IVehicleOwnerService
        //
        [HttpPost]
        [ActionName("AddVehicleOwner")]
        [Route("AddVehicleOwner")]
        [ResponseType(typeof(vehicle_ownerC))]
        public IHttpActionResult _AddVehicleOwner(dto_vehicle_owner_newC _dto)
        {
            var _service = _ServiceFactory.GetService<IVehicleOwnerService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Add(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("GetAllVehicleOwnersByFsTimeStamp")]
        [Route("GetAllVehicleOwnersByFsTimeStamp")]
        [ResponseType(typeof(List<vehicle_ownerC>))]
        public IHttpActionResult _GetAllVehicleOwnersByFsTimeStamp([FromUri] long fs_timestamp)
        {
            var _service = _ServiceFactory.GetService<IVehicleOwnerService>();
            _service.controller_key = _controller_key;
            var _resp = _service.GetAll(fs_timestamp).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpPost]
        [ActionName("UpdateVehicleOwner")]
        [Route("UpdateVehicleOwner")]
        [ResponseType(typeof(vehicle_ownerC))]
        public IHttpActionResult _UpdateVehicleOwner(dto_vehicle_owner_updateC _dto)
        {
            var _service = _ServiceFactory.GetService<IVehicleOwnerService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Update(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("DeleteVehicleOwner")]
        [Route("DeleteVehicleOwner")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult _DeleteVehicleOwner([FromUri] int id)
        {
            var _service = _ServiceFactory.GetService<IVehicleOwnerService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Delete(id).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }

        #endregion

        #region IVehicleService
        //
        [HttpPost]
        [ActionName("AddVehicle")]
        [Route("AddVehicle")]
        [ResponseType(typeof(vehicleC))]
        public IHttpActionResult _AddVehicle(dto_vehicle_newC _dto)
        {
            var _service = _ServiceFactory.GetService<IVehicleService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Add(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("GetAllVehiclesByFsTimeStamp")]
        [Route("GetAllVehiclesByFsTimeStamp")]
        [ResponseType(typeof(List<vehicleC>))]
        public IHttpActionResult _GetAllVehiclesByFsTimeStamp([FromUri] long fs_timestamp)
        {
            var _service = _ServiceFactory.GetService<IVehicleService>();
            _service.controller_key = _controller_key;
            var _resp = _service.GetAll(fs_timestamp).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpPost]
        [ActionName("UpdateVehicle")]
        [Route("UpdateVehicle")]
        [ResponseType(typeof(vehicleC))]
        public IHttpActionResult _UpdateVehicle(dto_vehicle_updateC _dto)
        {
            var _service = _ServiceFactory.GetService<IVehicleService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Update(_dto).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }
        //
        [HttpGet]
        [ActionName("DeleteVehicle")]
        [Route("DeleteVehicle")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult _DeleteVehicle([FromUri] int id)
        {
            var _service = _ServiceFactory.GetService<IVehicleService>();
            _service.controller_key = _controller_key;
            var _resp = _service.Delete(id).Result;
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return Ok(_resp);
        }

        #endregion
    }
}



