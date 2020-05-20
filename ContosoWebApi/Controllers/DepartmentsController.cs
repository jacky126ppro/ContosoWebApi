using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContosoWebApi.Model;
using Microsoft.Data.SqlClient;

namespace ContosoWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly ContosouniversityContext _context;

        public DepartmentsController(ContosouniversityContext context)
        {
            _context = context;
        }

        // GET: api/Departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartment()
        {
            return await _context.Department.ToListAsync();
        }

        // GET: api/Departments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            var department = await _context.Department.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return department;
        }

        // PUT: api/Departments/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartment(int id, Department department)
        {
            if (id != department.DepartmentId)
            {
                return BadRequest();
            }

            byte[] rowVersion = await _context.Department
                                   .Where(d => d.DepartmentId == id)
                                   .Select(c => c.RowVersion)
                                   .FirstOrDefaultAsync();
            department.RowVersion = rowVersion;
            department.DateModified = DateTime.Now;
            _context.Entry(department).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                //#region 預存程序
                //SqlParameter DepartmentID = new SqlParameter("@DepartmentID", id);
                //SqlParameter Name = new SqlParameter("@Name", department.Name);
                //SqlParameter Budget = new SqlParameter("@Budget", department.Budget);
                //SqlParameter StartDate = new SqlParameter("@StartDate", department.StartDate);
                //SqlParameter InstructorID = new SqlParameter("@InstructorID", department.InstructorId);
                //SqlParameter RowVersion_Original = new SqlParameter("@RowVersion_Original", rowVersion);
                //await _context.Database.ExecuteSqlRawAsync("EXEC Department_Update @DepartmentID, @Name, @Budget, @StartDate, @InstructorID, @RowVersion_Original", DepartmentID, Name, Budget, StartDate, InstructorID, RowVersion_Original);
                //#endregion
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Departments
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Department>> PostDepartment(Department department)
        {
            //#region Use Sp Department_Insert
            //SqlParameter Name = new SqlParameter("@Name", department.Name);
            //SqlParameter Budget = new SqlParameter("@Budget", department.Budget);
            //SqlParameter StartDate = new SqlParameter("@StartDate", department.StartDate);
            //SqlParameter InstructorID = new SqlParameter("@InstructorID", department.InstructorId);
            //department.DepartmentId =  _context.Department
            //                           .FromSqlRaw("EXEC dbo.Department_Insert @Name, @Budget, @StartDate, @InstructorID", Name, Budget, StartDate, InstructorID)
            //                           .Select(d => d.DepartmentId)
            //                           .ToList()
            //                           .First();
            //#endregion

            department.DateModified = DateTime.Now;

            _context.Department.Add(department);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDepartment", new { id = department.DepartmentId }, department);
        }

        // DELETE: api/Departments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Department>> DeleteDepartment(int id)
        {
            var department = await _context.Department.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            _context.Department.Remove(department);
            _context.SaveChanges();
            //#region Use stored procedure
            //SqlParameter departmentID = new SqlParameter("@DepartmentID", department.DepartmentId);
            //SqlParameter rowVersion = new SqlParameter("@RowVersion_Original", department.RowVersion);
            //_context.Database.ExecuteSqlRaw("execute Department_Delete @DepartmentID,@RowVersion_Original", departmentID, rowVersion);
            //#endregion

            return department;
        }

        private bool DepartmentExists(int id)
        {
            return _context.Department.Any(e => e.DepartmentId == id);
        }
    }
}
