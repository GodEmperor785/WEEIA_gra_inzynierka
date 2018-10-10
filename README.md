# WEEIA_gra_inzynierka

instrukcje dodawania do repo:

0. jeśli jeszcze nie ma repo - cd twoj/folder/z/solution - i potem git init
0,5. jesli to pierwszy commit - git remote add origin https://github.com/GodEmperor785/WEEIA_gra_inzynierka.git
1. git add wszystkie_pliki_do_commita
2. git commit
3. git push origin master

pamietaj o git remote update i git pull origin master (zawsze zanim zaczniesz coś robić - dla bezpieczeństwa)

jesli git rzuca błąd w stylu przy git push origin master:

  ! [rejected]        master -> master (fetch first)
  error: failed to push some refs to 'https://github.com/GodEmperor785/WEEIA_gra_inzynierka.git'
  hint: Updates were rejected because the remote contains work that you do
  hint: not have locally. This is usually caused by another repository pushing
  hint: to the same ref. You may want to first integrate the remote changes
  hint: (e.g., 'git pull ...') before pushing again.
  hint: See the 'Note about fast-forwards' in 'git push --help' for details.
 
 to zrób mu: git pull origin master
 i potem sprobuj znowu: git push origin master
