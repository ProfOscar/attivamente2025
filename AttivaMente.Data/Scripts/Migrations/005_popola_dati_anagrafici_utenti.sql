-- =========================================================
-- Popola Telefono, Indirizzo, Citta, Provincia, CAP
-- in modo realistico e VARIABILE per ogni utente
-- =========================================================

DECLARE @Vie TABLE (Nome NVARCHAR(100));
INSERT INTO @Vie VALUES
(N'Via Roma'),
(N'Via Garibaldi'),
(N'Via Cavour'),
(N'Via Dante'),
(N'Corso Italia'),
(N'Corso Francia'),
(N'Via Mazzini'),
(N'Via Verdi'),
(N'Via Manzoni'),
(N'Via Leopardi');

DECLARE @Citta TABLE (SiglaProv CHAR(2), Nome NVARCHAR(50), CAP NVARCHAR(5));
INSERT INTO @Citta VALUES
('MI',N'Milano','20100'),
('RM',N'Roma','00100'),
('TO',N'Torino','10100'),
('NA',N'Napoli','80100'),
('BO',N'Bologna','40100'),
('FI',N'Firenze','50100'),
('GE',N'Genova','16100'),
('VE',N'Venezia','30100'),
('PA',N'Palermo','90100'),
('BA',N'Bari','70100'),
('CN',N'Cuneo','12100'),
('CN',N'Alba','12051'),
('CN',N'Bra','12042'),
('CN',N'Fossano','12045'),
('CN',N'Saluzzo','12037'),
('AL',N'Alessandria','15100'),
('AT',N'Asti','14100'),
('NO',N'Novara','28100'),
('BS',N'Brescia','25100');

-- ID utenti da aggiornare
DECLARE @Id INT;

DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
SELECT Id
FROM Utenti
WHERE Telefono IS NULL
  AND Indirizzo IS NULL
  AND Citta IS NULL
  AND Provincia IS NULL
  AND CAP IS NULL;

OPEN cur;
FETCH NEXT FROM cur INTO @Id;

WHILE @@FETCH_STATUS = 0
BEGIN
    DECLARE @Via NVARCHAR(100);
    DECLARE @Civico INT;
    DECLARE @Sigla CHAR(2);
    DECLARE @NomeCitta NVARCHAR(50);
    DECLARE @Cap NVARCHAR(5);

    -- scegli 1 via casuale
    SELECT TOP 1 @Via = Nome FROM @Vie ORDER BY NEWID();
    SET @Civico = (ABS(CHECKSUM(NEWID())) % 200) + 1;

    -- scegli 1 città casuale (da cui derivano provincia e CAP coerenti)
    SELECT TOP 1
        @Sigla = SiglaProv,
        @NomeCitta = Nome,
        @Cap = CAP
    FROM @Citta
    ORDER BY NEWID();

    -- telefono: 3 + 9 cifre
    DECLARE @Tel NVARCHAR(20) =
        '3' +
        CAST((ABS(CHECKSUM(NEWID())) % 1000000000) + 100000000 AS NVARCHAR(20));

    UPDATE Utenti
    SET
        Telefono = @Tel,
        Indirizzo = @Via + N' ' + CAST(@Civico AS NVARCHAR(10)),
        Citta = @NomeCitta,
        Provincia = @Sigla,
        CAP = @Cap
    WHERE Id = @Id;

    FETCH NEXT FROM cur INTO @Id;
END

CLOSE cur;
DEALLOCATE cur;
