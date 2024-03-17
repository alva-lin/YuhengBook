import { myAxios } from '@/lib/api';
import { BookDetailDto, PaginatedList } from '@/lib/models';

const baseUrlSegment = 'book';

export async function getList(params: {
  page: number;
  pageSize: number;
  keyword?: string;
}): Promise<PaginatedList<BookDetailDto>> {
  const resp = await myAxios<PaginatedList<BookDetailDto>>(`${baseUrlSegment}`, {
    method: 'GET',
    params,
  });
  return resp.data;
}

export async function get(id: number): Promise<BookDetailDto> {
  const resp = await myAxios<BookDetailDto>(`${baseUrlSegment}/${id}`, {
    method: 'GET',
  });
  return resp.data;
}

export async function create(data: { name: string; description?: string }): Promise<number> {
  const resp = await myAxios<number>(`${baseUrlSegment}`, {
    method: 'POST',
    data,
  });

  return resp.data;
}

export async function update(
  id: number,
  data: {
    name: string;
    description?: string;
  }
): Promise<void> {
  const resp = await myAxios<void>(`${baseUrlSegment}/${id}`, {
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
