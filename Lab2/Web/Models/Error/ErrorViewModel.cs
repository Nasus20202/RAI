namespace Lab2.Web.Models.Error;

public class ErrorViewModel
{
    public int StatusCode { get; set; }
    public string Title { get; set; } = "Error";
    public string ErrorMessage { get; set; } = "An unexpected error occurred.";
}
