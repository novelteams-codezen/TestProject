using TestProject.Models;
using TestProject.Data;
using TestProject.Filter;
using TestProject.Entities;
using TestProject.Logger;
using Microsoft.AspNetCore.JsonPatch;
using System.Linq.Expressions;

namespace TestProject.Services
{
    /// <summary>
    /// The billingcycleService responsible for managing billingcycle related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting billingcycle information.
    /// </remarks>
    public interface IBillingCycleService
    {
        /// <summary>Retrieves a specific billingcycle by its primary key</summary>
        /// <param name="id">The primary key of the billingcycle</param>
        /// <returns>The billingcycle data</returns>
        BillingCycle GetById(Guid id);

        /// <summary>Retrieves a list of billingcycles based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of billingcycles</returns>
        List<BillingCycle> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new billingcycle</summary>
        /// <param name="model">The billingcycle data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(BillingCycle model);

        /// <summary>Updates a specific billingcycle by its primary key</summary>
        /// <param name="id">The primary key of the billingcycle</param>
        /// <param name="updatedEntity">The billingcycle data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, BillingCycle updatedEntity);

        /// <summary>Updates a specific billingcycle by its primary key</summary>
        /// <param name="id">The primary key of the billingcycle</param>
        /// <param name="updatedEntity">The billingcycle data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<BillingCycle> updatedEntity);

        /// <summary>Deletes a specific billingcycle by its primary key</summary>
        /// <param name="id">The primary key of the billingcycle</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The billingcycleService responsible for managing billingcycle related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting billingcycle information.
    /// </remarks>
    public class BillingCycleService : IBillingCycleService
    {
        private TestProjectContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the BillingCycle class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public BillingCycleService(TestProjectContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific billingcycle by its primary key</summary>
        /// <param name="id">The primary key of the billingcycle</param>
        /// <returns>The billingcycle data</returns>
        public BillingCycle GetById(Guid id)
        {
            var entityData = _dbContext.BillingCycle.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of billingcycles based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of billingcycles</returns>/// <exception cref="Exception"></exception>
        public List<BillingCycle> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetBillingCycle(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new billingcycle</summary>
        /// <param name="model">The billingcycle data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(BillingCycle model)
        {
            model.Id = CreateBillingCycle(model);
            return model.Id;
        }

        /// <summary>Updates a specific billingcycle by its primary key</summary>
        /// <param name="id">The primary key of the billingcycle</param>
        /// <param name="updatedEntity">The billingcycle data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, BillingCycle updatedEntity)
        {
            UpdateBillingCycle(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific billingcycle by its primary key</summary>
        /// <param name="id">The primary key of the billingcycle</param>
        /// <param name="updatedEntity">The billingcycle data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<BillingCycle> updatedEntity)
        {
            PatchBillingCycle(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific billingcycle by its primary key</summary>
        /// <param name="id">The primary key of the billingcycle</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteBillingCycle(id);
            return true;
        }
        #region
        private List<BillingCycle> GetBillingCycle(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.BillingCycle.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<BillingCycle>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(BillingCycle), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<BillingCycle, object>>(Expression.Convert(property, typeof(object)), parameter);
                if (sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase))
                {
                    result = result.OrderBy(lambda);
                }
                else if (sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase))
                {
                    result = result.OrderByDescending(lambda);
                }
                else
                {
                    throw new ApplicationException("Invalid sort order. Use 'asc' or 'desc'");
                }
            }

            var paginatedResult = result.Skip(skip).Take(pageSize).ToList();
            return paginatedResult;
        }

        private Guid CreateBillingCycle(BillingCycle model)
        {
            _dbContext.BillingCycle.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateBillingCycle(Guid id, BillingCycle updatedEntity)
        {
            _dbContext.BillingCycle.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteBillingCycle(Guid id)
        {
            var entityData = _dbContext.BillingCycle.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.BillingCycle.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchBillingCycle(Guid id, JsonPatchDocument<BillingCycle> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.BillingCycle.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.BillingCycle.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}