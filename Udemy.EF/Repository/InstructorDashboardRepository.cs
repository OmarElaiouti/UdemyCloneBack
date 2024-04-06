using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.Core.DTOs;
using Udemy.Core.Interfaces;
using Udemy.Core.Models;
using Udemy.Core.Models.UdemyContext;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;


namespace Udemy.EF.Repository
{
    public class InstructorDashboardRepository : BaseRepository<Course>, IInstructorDashboardRepository
    {
        private readonly UdemyContext _dbcontext;
        private readonly IBaseRepository<Course> _instructorDachboardRepository;
        //private readonly IWebHostEnvironment _hostEnvironment;


        public InstructorDashboardRepository(IBaseRepository<Course> instructorDachboardRepository,UdemyContext dbcontext) : base(dbcontext)
        {
            _dbcontext = dbcontext;
            _instructorDachboardRepository = instructorDachboardRepository;
        }



        
    }
}
