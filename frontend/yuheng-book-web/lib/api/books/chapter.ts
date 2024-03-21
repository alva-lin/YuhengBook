import { myAxios } from '@/lib/api';
import { ChapterDetailDto } from '@/lib/models';

const bookBaseUrlSegment = 'book';
const baseUrlSegment = 'chapter';

export async function get({
  bookId,
  order,
}: {
  bookId: number;
  order: number;
}): Promise<ChapterDetailDto> {
  const resp = await myAxios<ChapterDetailDto>(
    `${bookBaseUrlSegment}/${bookId}/${baseUrlSegment}/${order}`,
    {
      method: 'GET',
    }
  );
  return resp.data;
}

export async function create(data: {
  bookId: number;
  order?: number;
  title: string;
  content: string;
}): Promise<number> {
  const resp = await myAxios<number>(`${bookBaseUrlSegment}/${data.bookId}/${baseUrlSegment}`, {
    method: 'POST',
    data,
  });
  return resp.data;
}

export async function update(
  id: number,
  data: {
    title: string;
  }
): Promise<void> {
  const resp = await myAxios<void>(`${baseUrlSegment}/${id}`, {
    method: 'PUT',
    data,
  });
  return resp.data;
}

export async function updateDetail(
  id: number,
  data: {
    content: string;
  }
): Promise<void> {
  const resp = await myAxios<void>(`${baseUrlSegment}/${id}/detail`, {
    method: 'PUT',
    data,
  });
  return resp.data;
}

export async function remove(id: number): Promise<void> {
  const resp = await myAxios<void>(`${baseUrlSegment}/${id}`, {
    method: 'DELETE',
  });
  return resp.data;
}
