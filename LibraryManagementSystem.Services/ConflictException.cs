namespace LibraryManagementSystem.Services;

public sealed class ConflictException(string message, int memberId) : Exception(message);
