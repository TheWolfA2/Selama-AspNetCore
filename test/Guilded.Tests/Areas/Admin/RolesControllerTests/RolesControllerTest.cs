﻿using Guilded.Areas.Admin.Controllers;
using Guilded.Areas.Admin.DAL;
using Guilded.Tests.Controllers;
using Microsoft.Extensions.Logging;
using Moq;

namespace Guilded.Tests.Areas.Admin.RolesControllerTests
{
    public class RolesControllerTest : ControllerTest<RolesController>
    {
        protected Mock<IRolesDataContext> MockAdminDataContext;

        protected override RolesController SetUpController()
        {
            MockAdminDataContext = new Mock<IRolesDataContext>();

            return new RolesController(
                MockAdminDataContext.Object,
                MockLoggerFactory.Object
            );
        }
    }
}