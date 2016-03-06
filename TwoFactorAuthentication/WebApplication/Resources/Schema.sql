
create table if not exists USER_TWOFACTOR
(
    UserId nvarchar(255) primary key,
    Secret nvarchar(200) not null
);

