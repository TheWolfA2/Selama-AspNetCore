﻿using Guilded.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Guilded.Tests.Controllers
{


    [TestFixture]
    public abstract class ControllerTest<TController>
        where TController : BaseController
    {
        protected virtual Expression<Func<TController, Func<int, Task<IActionResult>>>> AsyncActionToTest { get; }

        protected TController Controller { get; private set; }

        protected Mock<IUrlHelper> MockUrlHelper { get; private set; }

        protected Mock<ITempDataDictionary> MockTempData { get; private set; }

        protected Mock<ControllerContext> MockControllerContext { get; private set; }

        protected Mock<ActionContext> MockActionContext { get; private set; }

        protected Mock<RouteData> MockRouteData { get; private set; }

        protected Mock<ControllerActionDescriptor> MockActionDescriptor { get; private set; }

        protected Mock<HttpContext> MockHttpContext { get; private set; }

        protected Mock<HttpRequest> MockHttpRequest { get; private set; }

        protected Mock<HttpResponse> MockHttpResponse { get; private set; }

        protected Mock<ClaimsPrincipal> MockUser { get; private set; }

        protected Mock<IIdentity> MockIdentity { get; private set; }

        protected Mock<ILoggerFactory> MockLoggerFactory { get; private set; }

        protected Mock<ILogger> MockLogger { get; private set; }

        protected abstract TController SetUpController();

        [SetUp]
        public void BaseSetUp()
        {
            InitializeLogging();

            InitializeHttpContext();

            InitializeControllerContext();

            MockUrlHelper = SetUpUrlHelper();
            MockTempData = SetUpTempData();

            Controller = SetUpController();
            Controller.ControllerContext = MockControllerContext.Object;
            Controller.Url = MockUrlHelper.Object;
            Controller.TempData = MockTempData.Object;

            AdditionalSetUp();
        }

        /// <summary>
        /// Sets up and returns the mocked user for the controller.
        /// </summary>
        /// <returns></returns>
        protected virtual Mock<ClaimsPrincipal> SetUpUser() => new Mock<ClaimsPrincipal>();

        protected virtual Mock<IIdentity> SetUpUserIdentity() => new Mock<IIdentity>();

        /// <summary>
        /// Sets up and returns the mocked <see cref="HttpContext"/> to be used in the tests.
        /// </summary>
        /// <returns></returns>
        protected virtual Mock<HttpContext> SetUpHttpContext()
        {
            return new Mock<HttpContext>();
        }

        /// <summary>
        /// Sets up and returns the mocked <see cref="HttpResponse"/> to be used in the tests.
        /// </summary>
        /// <returns></returns>
        protected virtual Mock<HttpResponse> SetUpHttpResponse()
        {
            return new Mock<HttpResponse>();
        }

        /// <summary>
        /// Sets up and returns the mocked <see cref="HttpRequest"/> to be used in the tests.
        /// </summary>
        /// <returns></returns>
        protected virtual Mock<HttpRequest> SetUpHttpRequest()
        {
            return new Mock<HttpRequest>();
        }

        /// <summary>
        /// Sets up and returns the mocked <see cref="RouteData"/> to be used in the tests.
        /// </summary>
        /// <returns></returns>
        protected virtual Mock<RouteData> SetUpRouteData()
        {
            return new Mock<RouteData>();
        }

        /// <summary>
        /// Sets up and returns the mocked <see cref="ControllerActionDescriptor"/> to be
        /// used in the tests.
        /// </summary>
        /// <returns></returns>
        protected virtual Mock<ControllerActionDescriptor> SetUpActionDescriptor() => new Mock<ControllerActionDescriptor>();

        /// <summary>
        /// Sets up and returns the mocked <see cref="IUrlHelper"/> to be used in the tests.
        /// </summary>
        /// <returns></returns>
        protected virtual Mock<IUrlHelper> SetUpUrlHelper() => new Mock<IUrlHelper>();

        protected virtual Mock<ITempDataDictionary> SetUpTempData()
        {
            return new Mock<ITempDataDictionary>();
        }

        /// <summary>
        /// Perform anys additional set up necessary for the tests, after the controller is initalized.
        /// </summary>
        protected virtual void AdditionalSetUp()
        {
            // Nothing to do in base class
        }

        protected void MockUserIdIsThis(string userId)
        {
            MockUser.Setup(u => u.Claims)
                .Returns(new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userId)
                });
        }

        private void InitializeLogging()
        {
            MockLoggerFactory = new Mock<ILoggerFactory>();
            MockLogger = new Mock<ILogger>();

            MockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(MockLogger.Object);
        }

        private void InitializeHttpContext()
        {
            InitializeHttpContextProperties();

            MapHttpContextPropertiesToMockObjects();
        }

        private void InitializeHttpContextProperties()
        {
            MockIdentity = SetUpUserIdentity();

            MockUser = SetUpUser();
            MockUser.Setup(u => u.Identity).Returns(MockIdentity.Object);

            MockHttpContext = SetUpHttpContext();
            MockHttpRequest = SetUpHttpRequest();
            MockHttpResponse = SetUpHttpResponse();
            MockHttpResponse.SetupSet(r => r.StatusCode = It.IsAny<int>()).Verifiable();
        }

        private void MapHttpContextPropertiesToMockObjects()
        {
            MockHttpContext.Setup(ctxt => ctxt.User).Returns(MockUser.Object);
            MockHttpContext.Setup(ctxt => ctxt.Request).Returns(MockHttpRequest.Object);
            MockHttpContext.Setup(ctxt => ctxt.Response).Returns(MockHttpResponse.Object);
        }

        private void InitializeControllerContext()
        {
            InitializeActionContext();
            MockControllerContext = SetupControllerContext();
        }

        private void InitializeActionContext()
        {
            MockRouteData = SetUpRouteData();
            MockActionDescriptor = SetUpActionDescriptor();
            MockActionContext = SetupActionContext();
        }

        private Mock<ActionContext> SetupActionContext()
        {
            return new Mock<ActionContext>(MockHttpContext.Object, MockRouteData.Object, MockActionDescriptor.Object);
        }

        private Mock<ControllerContext> SetupControllerContext()
        {
            return new Mock<ControllerContext>(MockActionContext.Object);
        }
    }
}