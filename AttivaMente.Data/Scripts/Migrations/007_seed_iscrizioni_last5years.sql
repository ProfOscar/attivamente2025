DECLARE @AnnoCorrente INT = YEAR(GETDATE());
DECLARE @Start INT = @AnnoCorrente - 4;

-- Per ogni anno, iscrivo una percentuale di utenti
DECLARE @Anno INT = @Start;

WHILE @Anno <= @AnnoCorrente
BEGIN
    -- Campione variabile per anno (es. 60%..80%)
    DECLARE @Perc INT = 60 + (ABS(CHECKSUM(NEWID())) % 21); -- 60..80

    ;WITH Campione AS
    (
        SELECT TOP (@Perc) PERCENT Id
        FROM Utenti
        ORDER BY NEWID()
    )
    INSERT INTO Iscrizioni (UtenteId, Anno, Tipo, DataIscrizione)
    SELECT
        c.Id,
        @Anno,
        CASE
            WHEN EXISTS (SELECT 1 FROM Iscrizioni i WHERE i.UtenteId = c.Id AND i.Anno = @Anno-1)
                THEN N'Rinnovo'
            ELSE N'Nuova'
        END,
        DATEFROMPARTS(@Anno, 1 + (ABS(CHECKSUM(NEWID())) % 12), 1 + (ABS(CHECKSUM(NEWID())) % 27))
    FROM Campione c
    WHERE NOT EXISTS (SELECT 1 FROM Iscrizioni i WHERE i.UtenteId = c.Id AND i.Anno = @Anno);

    SET @Anno += 1;
END


