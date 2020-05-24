using FleetManager.Shared.Core;
using FleetManager.Shared.dto;
using FleetManager.Shared.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;


namespace FleetManager.Server.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LoginController : ApiController
    {
        string _controller_key;
        static IServiceFactory _ServiceFactory;
        public LoginController()
        {
            _ServiceFactory = new ServiceFactory();
            _controller_key = Guid.NewGuid().ToString();
            if (datam.DATA_CONTROLLER_MODEL_STATE.IndexOfKey(_controller_key) == -1)
            {
                datam.DATA_CONTROLLER_MODEL_STATE.Add(_controller_key, this.ModelState);
            }
        }
        public LoginController(IServiceFactory serviceFactory)
        {
            _ServiceFactory = serviceFactory;
            _controller_key = Guid.NewGuid().ToString();
            if (datam.DATA_CONTROLLER_MODEL_STATE.IndexOfKey(_controller_key) == -1)
            {
                datam.DATA_CONTROLLER_MODEL_STATE.Add(_controller_key, this.ModelState);
            }
        }
        #region ILoginService
        //
        [HttpPost]
        [ActionName("LoginAdmin")]
        [Route("LoginAdmin")]
        [ResponseType(typeof(dto_pc_userC))]
        public IHttpActionResult _LoginUser(dto_login _dto)
        {
            try
            {
                var _service = _ServiceFactory.GetService<ILoginService>();
                if(_service==null)
                {
                    return BadRequest("null serveice");
                }
                _service.controller_key = _controller_key;
                var _resp = _service.LoginAdmin(_dto).Result;
                if (!this.ModelState.IsValid)
                {
                    return BadRequest(this.ModelState);
                }
                return Ok(_resp);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        #endregion

    }
}
