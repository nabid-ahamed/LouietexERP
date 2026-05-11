using LouietexERP.Data;
using LouietexERP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LouietexERP.Controllers
{
    [Authorize(Roles = SD.Role_SuperAdmin + "," + SD.Role_Admin)]
    public class AdminToolsController() : Controller;
}
