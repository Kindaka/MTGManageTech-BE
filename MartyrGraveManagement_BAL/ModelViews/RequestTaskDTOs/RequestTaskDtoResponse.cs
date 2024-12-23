using MartyrGraveManagement_BAL.ModelViews.RequestMaterialDTOs;
using MartyrGraveManagement_BAL.ModelViews.TaskDTOs;

namespace MartyrGraveManagement_BAL.ModelViews.RequestTaskDTOs
{
    public class RequestTaskDtoResponse
    {
        public int RequestTaskId { get; set; }
        public int StaffId { get; set; }
        public string? Fullname { get; set; }
        public string? Phone { get; set; }
        public int RequestId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public string? ImageWorkSpace { get; set; }
        public List<TaskImageDtoResponse> TaskImages { get; set; } = new List<TaskImageDtoResponse>();
        public string? Reason { get; set; }


        // Các thuộc tính mới
        public string? ServiceName { get; set; }
        public string? ServiceDescription { get; set; }
        public string? GraveLocation { get; set; }

        public List<RequestMaterialDtoResponse> Materials { get; set; } = new List<RequestMaterialDtoResponse> { };
    }
}
