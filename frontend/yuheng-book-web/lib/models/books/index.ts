export interface BookInfoDto {
  id: number;
  name: string;
  description?: string;
  chapterCount: number;
  lastChapter?: ChapterInfoDto;
}

export interface BookDetailDto {
  id: number;
  name: string;
  description?: string;
  chapterCount: number;
  lastChapter?: ChapterInfoDto;
  chapters: ChapterInfoDto[];
}

export interface ChapterInfoDto {
  order: number;
  title: string;
}

export interface ChapterDetailDto {
  bookId: number;
  order: number;
  title: string;
  content: string;
}
