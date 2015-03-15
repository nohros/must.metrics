if exists (select name from sys.tables where name = 'mtc_tag_serie')
begin
  if (select count(*) from mtc_tag_serie) > 0
  begin
    raiserror('A tabela [mtc_tag_serie] contem dados e não pode ser excluida.',16,1)
  end
  drop table mtc_tag_serie
end
go

create table mtc_tag_serie (
  serie_id bigint,
  tag_id bigint
)

alter table mtc_tag_serie
add constraint FK_mtc_tag_serie_serie
foreign key (
  serie_id
) references mtc_serie (
  serie_id
)

alter table mtc_tag_serie
add constraint FK_mtc_tag_serie_tag
foreign key (
  tag_id
) references mtc_tag (
  tag_id
)

create unique nonclustered index IX_mtc_tag_serie_natural
on mtc_tag_serie (
  tag_id, serie_id
)