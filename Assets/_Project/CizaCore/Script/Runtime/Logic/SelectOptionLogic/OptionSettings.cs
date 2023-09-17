using System;
using System.Collections.Generic;
using UnityEngine;

namespace CizaCore
{
	[Serializable]
	public abstract class OptionSettings<TOption> where TOption : IOptionReadModel
	{
		[SerializeField]
		private OptionColumn[] _optionColumns;

		public int OptionKeysLength
		{
			get
			{
				if (_optionColumns is null || _optionColumns.Length <= 0)
					return 0;
				var options = _optionColumns[0].Options;
				if (options is null || options.Length <= 0)
					return 0;
				return options.Length;
			}
		}

		public IOptionColumn[] OptionColumns => GetOptionColumns().ToArray();

		public TOption[] Options => GetOptions().ToArray();

		public TOption[] OptionsIncludeNull => GetOptionsIncludeNull().ToArray();

		private List<IOptionColumn> GetOptionColumns()
		{
			var optionColumns = new List<IOptionColumn>();
			foreach (var optionColumn in _optionColumns)
				optionColumns.Add(new OptionColumnImp(optionColumn.GetOptionKeys(OptionKeysLength).ToArray()));
			return optionColumns;
		}

		private List<TOption> GetOptions()
		{
			var options = new List<TOption>();
			foreach (var optionColumn in _optionColumns)
			{
				foreach (var option in optionColumn.Options)
				{
					if (option is null)
						continue;

					if (Contains(options, option))
						continue;

					options.Add(option);
				}
			}

			return options;
		}

		private List<TOption> GetOptionsIncludeNull()
		{
			var optionsIncludeNull = new List<TOption>();
			foreach (var optionColumn in _optionColumns)
				foreach (var option in optionColumn.Options)
					optionsIncludeNull.Add(option);

			return optionsIncludeNull;
		}

		private bool Contains(List<TOption> options, TOption option)
		{
			foreach (var optionReadModel in options)
				if (optionReadModel.Key == option.Key)
					return true;

			return false;
		}

		private class OptionColumnImp : IOptionColumn
		{
			public OptionColumnImp(string[] optionKeys) =>
				OptionKeys = optionKeys;

			public string[] OptionKeys { get; }
		}

		[Serializable]
		private class OptionColumn
		{
			[SerializeField]
			private TOption[] _options;

			public TOption[] Options => _options;

			public List<string> GetOptionKeys(int length)
			{
				var optionKeys = new List<string>();
				for (var i = 0; i < length; i++)
				{
					if (i >= _options.Length)
					{
						optionKeys.Add(string.Empty);
						continue;
					}

					var menuOption = _options[i];
					if (menuOption is null)
					{
						optionKeys.Add(string.Empty);
						continue;
					}

					optionKeys.Add(menuOption.Key);
				}

				return optionKeys;
			}
		}
	}
}
