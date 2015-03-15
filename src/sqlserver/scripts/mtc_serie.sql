if exists (select name from sys.tables where name = 'mtc_serie')
begin
  if (select count(*) from mtc_serie) > 0
  begin
    raiserror('A tabela [mtc_serie] contem dados e não pode ser excluida.',16,1)
  end
  drop table mtc_serie
end
go

create table mtc_serie (
  serie_id bigint identity(1,1) not null,
  serie_name varchar(256) not null,
  serie_hash int not null,
  serie_tags_count int not null
)

alter table mtc_serie
add constraint PK_mtc_serie
primary key (
  serie_id
)

create index IX_mtc_serie_similar
on mtc_serie (
  serie_name, serie_hash, serie_tags_count
)