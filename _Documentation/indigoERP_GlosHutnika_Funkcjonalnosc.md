# Głos Hutnika — funkcjonalność

W zakresie „prywatne zgoda na Głos Hutnika” aplikacja wybiera pracowników z aktywnym zatrudnieniem i ich adresy służbowe. Adresy prywatne są uwzględniane, gdy pracownik ma zgodę na Głos Hutnika. Identyczne adresy, porównywane bez rozróżniania wielkości liter i otaczających spacji, są używane tylko raz w wysyłce. Operator wybiera osoby i widzi ich adresy w jednym wierszu. Reguła wymaga wdrożenia skryptu `indigoERP20261113150000.sql` oraz aktualizacji API i Blazora.
