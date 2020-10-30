## Integration Tests

Integration tests (we are defining) are where we are hitting the Website.

Unit Tests are directly hitting lower level functions (eg Db.cs)

## Connection String

These tests use the CustomWebApplicationFactory where a db connection is needed

This reads config settings off the refereneced PasswordPostgres.Web projects appsettings.Development.json file

## More Information

I've written an article on davemateer.com on how I've setup testing.
