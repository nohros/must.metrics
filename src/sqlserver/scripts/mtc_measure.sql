if exists (select name from sys.tables where name = 'mtc_measure')
begin
  if (select count(*) from mtc_measure) > 0
  begin
    raiserror('A tabela [mtc_measure] contem dados e não pode ser excluida.',16,1)
  end
  drop table mtc_measure
end
go

create table mtc_measure (
  serie_id bigint not null,
  measure_value float(24) not null,
  measure_time datetime2(0) not null
)

create clustered index IX_mtc_measure_cluster
on mtc_measure (
  measure_time
)

alter table mtc_measure
add constraint FK_mtc_measure_serie
foreign key (
  serie_id
) references mtc_serie (
  serie_id
)