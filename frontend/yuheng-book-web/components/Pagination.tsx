import { useEffect, useState } from 'react';

import { Button, Select } from '@mantine/core';
import { useToggle } from '@mantine/hooks';

export default function MyPagination(props: {
  count: number;
  page?: number;
  pageSize?: number;
  showOrder?: boolean;
  orderBy?: 'asc' | 'desc';
  onChange?: (page: number, pageSize: number, orderBy: 'asc' | 'desc') => void;
}) {
  const [page, setPage] = useState(props.page ?? 1);
  const [pageSize, setPageSize] = useState(props.pageSize ?? 30);

  const totalPage = Math.ceil(props.count / pageSize);

  const [sort, toggleSort] = useToggle(['asc', 'desc'] as const);
  const pageArray = Array.from({ length: totalPage }, (_, i) => ({
    value: (i + 1).toString(),
    label: `第 ${i + 1} 页`,
  }));

  useEffect(() => {
    toggleSort(props.orderBy ?? 'asc');
  }, []);

  useEffect(() => {
    const { onChange } = props;
    if (onChange) {
      onChange(page, pageSize, sort);
    }
  }, [page, pageSize, sort]);

  return (
    <div className="flex justify-center items-center gap-2">
      {props.showOrder && (
        <Button variant="default" onClick={() => toggleSort()}>
          {sort === 'asc' ? '正序' : '倒序'}
        </Button>
      )}
      <Select
        w={120}
        data={pageArray}
        value={page.toString()}
        onChange={(value) => {
          setPage(parseInt(value ?? '10', 10));
        }}
      />
      <Select
        w={120}
        data={['10', '30', '50', '100'].map((value) => ({
          value,
          label: `${value} 章 / 页`,
        }))}
        defaultValue="30"
        value={pageSize.toString()}
        onChange={(value) => {
          setPageSize(parseInt(value ?? '10', 10));
        }}
      />
    </div>
  );
}
