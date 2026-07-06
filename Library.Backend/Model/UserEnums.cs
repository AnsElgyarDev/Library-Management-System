namespace Library.Core.Model;

public enum Role
{
    Admin = 1,
    User
}
public enum BorrowResultStatus
{
    Success = 1,
    BookNotFound,
    BookNotAvailable,
    UserNotFound,
    UserLimitExceeded
}
public enum ReturnResultStatus
{
    Success = 1,
    BookNotFound,
    BookAvailable,
    UserHasNoBorrowedBooks,
    UserNotFound,
    UserBookNotFound
}