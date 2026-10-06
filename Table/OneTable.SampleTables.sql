
INSERT INTO app."WorkNotice"
    ("WorkNoticeCode", "WorkNoticeName", "WorkNoticeDate", "IsEditAble", "Note", "StatusId", "CreatedById")
VALUES
    ('WN-001', 'Ramadan office hours',     DATE '2026-03-01', TRUE,  'Shorter hours during Ramadan.', 1, 1),
    ('WN-002', 'Payroll cutoff reminder',  DATE '2026-03-20', TRUE,  'Submit overtime before cutoff.', 1, 1),
    ('WN-003', 'Annual medical checkup',   DATE '2026-04-10', FALSE, 'Plant staff — schedule with HR.', 1, 1),
    ('WN-004', 'Independence day holiday', DATE '2026-08-17', TRUE,  'National holiday — office closed.', 1, 1);
