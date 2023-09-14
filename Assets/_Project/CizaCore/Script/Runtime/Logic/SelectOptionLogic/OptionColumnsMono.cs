using System;
using System.Collections.Generic;
using UnityEngine;

namespace CizaCore
{
	public class OptionColumnsMono<TOption> : MonoBehaviour where TOption : IOptionReadModel
	{
		[SerializeField]
		private int _optionKeysLength = 2;

		[SerializeField]
		private OptionColumn[] _optionColumns;

		public IOptionColumn[] OptionColumns => GetOptionColumns().ToArray();

		public IOptionReadModel[] OptionReadModels => GetOptionReadModels().ToArray();

		private List<IOptionColumn> GetOptionColumns()
		{
			var optionColumns = new List<IOptionColumn>();
			foreach (var optionColumn in _optionColumns)
				optionColumns.Add(new OptionColumnImp(optionColumn.GetOptionKeys(_optionKeysLength).ToArray()));
			return optionColumns;
		}

		private List<IOptionReadModel> GetOptionReadModels()
		{
			var optionReadModels = new List<IOptionReadModel>();
			foreach (var optionColumn in _optionColumns)
			{
				foreach (var option in optionColumn.Options)
				{
					if (option is null)
						continue;

					if (Contains(optionReadModels, option))
						continue;

					optionReadModels.Add(option);
				}
			}

			return optionReadModels;
		}

		private bool Contains(List<IOptionReadModel> optionReadModels, TOption option)
		{
			foreach (var optionReadModel in optionReadModels)
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
					optionKeys.Add(menuOption.Key);
				}

				return optionKeys;
			}
		}
	}
}
